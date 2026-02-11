using FSH.Framework.Core.Context;
using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using Mediator;
using System.Security.Claims;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Eventing.Outbox;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Identity.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Identity.Features.v1.Tokens.TokenGeneration;

public sealed class GenerateTokenCommandHandler
    : ICommandHandler<GenerateTokenCommand, TokenResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ISecurityAudit _securityAudit;
    private readonly IRequestContext _requestContext;
    private readonly IOutboxStore _outboxStore;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _multiTenantContextAccessor;
    private readonly ISessionService _sessionService;
    private readonly ILogger<GenerateTokenCommandHandler> _logger;
    private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GenerateTokenCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        ISecurityAudit securityAudit,
        IRequestContext requestContext,
        IOutboxStore outboxStore,
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        ISessionService sessionService,
        ILogger<GenerateTokenCommandHandler> logger,
        IMultiTenantStore<AppTenantInfo> tenantStore,
        IServiceScopeFactory serviceScopeFactory)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _securityAudit = securityAudit;
        _requestContext = requestContext;
        _outboxStore = outboxStore;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _sessionService = sessionService;
        _logger = logger;
        _tenantStore = tenantStore;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async ValueTask<TokenResponse> Handle(
        GenerateTokenCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var ip = _requestContext.IpAddress ?? "unknown";
        var ua = _requestContext.UserAgent ?? "unknown";
        var clientId = _requestContext.ClientId;

        // If no tenant header is provided, auto-discover the user's tenant
        var currentTenant = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
        if (currentTenant is null || string.IsNullOrWhiteSpace(currentTenant.Id))
        {
            return await AuthenticateWithTenantDiscoveryAsync(request, ip, ua, clientId, cancellationToken);
        }

        return await AuthenticateInCurrentScopeAsync(request, ip, ua, clientId, cancellationToken);
    }

    // Normal path: tenant was provided via the request header
    private async Task<TokenResponse> AuthenticateInCurrentScopeAsync(
        GenerateTokenCommand request,
        string ip, string ua, string? clientId,
        CancellationToken cancellationToken)
    {
        var identityResult = await _identityService
            .ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

        if (identityResult is null)
        {
            await _securityAudit.LoginFailedAsync(
                subjectIdOrName: request.Email,
                clientId: clientId!,
                reason: "InvalidCredentials",
                ip: ip,
                ct: cancellationToken);

            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var (subject, claims) = identityResult.Value;

        await _securityAudit.LoginSucceededAsync(
            userId: subject,
            userName: claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
            clientId: clientId!,
            ip: ip,
            userAgent: ua,
            ct: cancellationToken);

        var token = await _tokenService.IssueAsync(subject, claims, cancellationToken);

        await _identityService.StoreRefreshTokenAsync(subject, token.RefreshToken, token.RefreshTokenExpiresAt, cancellationToken);

        try
        {
            await _sessionService.CreateSessionAsync(
                subject,
                Sha256Short(token.RefreshToken),
                ip, ua,
                token.RefreshTokenExpiresAt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create user session for user {UserId}. Login will continue without session tracking.", subject);
        }

        var fingerprint = Sha256Short(token.AccessToken);
        await _securityAudit.TokenIssuedAsync(
            userId: subject,
            userName: claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
            clientId: clientId!,
            tokenFingerprint: fingerprint,
            expiresUtc: token.AccessTokenExpiresAt,
            ct: cancellationToken);

        await EnqueueTokenGeneratedEventAsync(request, subject, claims, clientId, ip, ua, fingerprint, token, cancellationToken);

        return token;
    }

    // Auto-discovery path: no tenant header — find the tenant by searching all active tenants
    private async Task<TokenResponse> AuthenticateWithTenantDiscoveryAsync(
        GenerateTokenCommand request,
        string ip, string ua, string? clientId,
        CancellationToken ct)
    {
        _logger.LogInformation("No tenant header provided for {Email}. Starting tenant auto-discovery.", request.Email);

        var allTenants = (await _tenantStore.GetAllAsync()).Where(t => t.IsActive).ToList();

        foreach (var tenantInfo in allTenants)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);

            // Check if the user exists in this tenant (cheap lookup, no password check)
            var resolver = scope.ServiceProvider.GetRequiredService<IUserTenantResolver>();
            if (!await resolver.UserExistsInTenantAsync(request.Email, ct))
                continue;

            _logger.LogInformation("User {Email} found in tenant {TenantId}. Authenticating.", request.Email, tenantInfo.Id);

            // User is in this tenant — do the full auth flow in this scope
            var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var identityResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, ct);

            if (identityResult is null)
            {
                await AuditLoginFailedSafeAsync(request.Email, clientId, "InvalidCredentials", ip, ct);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var (subject, claims) = identityResult.Value;

            var securityAudit = scope.ServiceProvider.GetRequiredService<ISecurityAudit>();
            await securityAudit.LoginSucceededAsync(
                userId: subject,
                userName: claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
                clientId: clientId!,
                ip: ip,
                userAgent: ua,
                ct: ct);

            var token = await _tokenService.IssueAsync(subject, claims, ct);

            await identityService.StoreRefreshTokenAsync(subject, token.RefreshToken, token.RefreshTokenExpiresAt, ct);

            try
            {
                var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                await sessionService.CreateSessionAsync(
                    subject,
                    Sha256Short(token.RefreshToken),
                    ip, ua,
                    token.RefreshTokenExpiresAt,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create session for user {UserId} during tenant-discovery login.", subject);
            }

            var fingerprint = Sha256Short(token.AccessToken);
            await securityAudit.TokenIssuedAsync(
                userId: subject,
                userName: claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
                clientId: clientId!,
                tokenFingerprint: fingerprint,
                expiresUtc: token.AccessTokenExpiresAt,
                ct: ct);

            var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
            await EnqueueTokenGeneratedEventInScopeAsync(request, subject, claims, tenantInfo.Id, clientId, ip, ua, fingerprint, token, outboxStore, ct);

            return token;
        }

        // No tenant found for this user
        await AuditLoginFailedSafeAsync(request.Email, clientId, "InvalidCredentials", ip, ct);
        throw new UnauthorizedAccessException("Invalid credentials.");
    }

    private async Task AuditLoginFailedSafeAsync(string email, string? clientId, string reason, string ip, CancellationToken ct)
    {
        try
        {
            await _securityAudit.LoginFailedAsync(
                subjectIdOrName: email,
                clientId: clientId!,
                reason: reason,
                ip: ip,
                ct: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to audit login failure for {Email}.", email);
        }
    }

    private async Task EnqueueTokenGeneratedEventAsync(
        GenerateTokenCommand request,
        string subject,
        IEnumerable<Claim> claims,
        string? clientId,
        string ip, string ua,
        string fingerprint,
        TokenResponse token,
        CancellationToken ct)
    {
        var tenantId = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;
        var correlationId = Guid.NewGuid().ToString();

        var integrationEvent = new TokenGeneratedIntegrationEvent(
            Id: Guid.NewGuid(),
            OccurredOnUtc: DateTime.UtcNow,
            TenantId: tenantId,
            CorrelationId: correlationId,
            Source: "Identity",
            UserId: subject,
            Email: request.Email,
            ClientId: clientId!,
            IpAddress: ip,
            UserAgent: ua,
            TokenFingerprint: fingerprint,
            AccessTokenExpiresAtUtc: token.AccessTokenExpiresAt);

        await _outboxStore.AddAsync(integrationEvent, ct).ConfigureAwait(false);
    }

    private static async Task EnqueueTokenGeneratedEventInScopeAsync(
        GenerateTokenCommand request,
        string subject,
        IEnumerable<Claim> claims,
        string? tenantId,
        string? clientId,
        string ip, string ua,
        string fingerprint,
        TokenResponse token,
        IOutboxStore outboxStore,
        CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();

        var integrationEvent = new TokenGeneratedIntegrationEvent(
            Id: Guid.NewGuid(),
            OccurredOnUtc: DateTime.UtcNow,
            TenantId: tenantId,
            CorrelationId: correlationId,
            Source: "Identity",
            UserId: subject,
            Email: request.Email,
            ClientId: clientId!,
            IpAddress: ip,
            UserAgent: ua,
            TokenFingerprint: fingerprint,
            AccessTokenExpiresAtUtc: token.AccessTokenExpiresAt);

        await outboxStore.AddAsync(integrationEvent, ct).ConfigureAwait(false);
    }

    private static string Sha256Short(string value)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash.AsSpan(0, 8));
    }
}
