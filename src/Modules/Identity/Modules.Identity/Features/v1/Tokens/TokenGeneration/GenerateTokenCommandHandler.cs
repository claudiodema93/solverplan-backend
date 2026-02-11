using FSH.Framework.Core.Context;
using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using Mediator;
using System.Security.Claims;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Eventing.Outbox;
using FSH.Framework.Shared.Constants;
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
            await AuditLoginFailedSafeAsync(request.Email, clientId, "InvalidCredentials", ip, cancellationToken);
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

        // Add tenants claim: scan all tenants to find where this user exists
        var allAccessibleIds = await GetAllAccessibleTenantIdsAsync(request.Email, cancellationToken);
        var enrichedClaims = EnrichWithTenantsClaim(claims, allAccessibleIds);

        var token = await _tokenService.IssueAsync(subject, enrichedClaims, cancellationToken);

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
            userName: enrichedClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
            clientId: clientId!,
            tokenFingerprint: fingerprint,
            expiresUtc: token.AccessTokenExpiresAt,
            ct: cancellationToken);

        await EnqueueTokenGeneratedEventAsync(request, subject, enrichedClaims, clientId, ip, ua, fingerprint, token, cancellationToken);

        return token;
    }

    // Auto-discovery path: no tenant header — find the tenant by scanning all active tenants in one pass
    private async Task<TokenResponse> AuthenticateWithTenantDiscoveryAsync(
        GenerateTokenCommand request,
        string ip, string ua, string? clientId,
        CancellationToken ct)
    {
        _logger.LogInformation("No tenant header provided for {Email}. Starting tenant auto-discovery.", request.Email);

        var allTenants = (await _tenantStore.GetAllAsync()).Where(t => t.IsActive).ToList();

        // One pass: collect all accessible tenant IDs and remember the first match for auth
        AppTenantInfo? authTenant = null;
        var allAccessibleIds = new List<string>();

        foreach (var tenantInfo in allTenants)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);

            var resolver = scope.ServiceProvider.GetRequiredService<IUserTenantResolver>();
            if (!await resolver.UserExistsInTenantAsync(request.Email, ct))
                continue;

            allAccessibleIds.Add(tenantInfo.Id!);
            authTenant ??= tenantInfo; // First tenant found is used for authentication
        }

        if (authTenant is null)
        {
            await AuditLoginFailedSafeAsync(request.Email, clientId, "InvalidCredentials", ip, ct);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        _logger.LogInformation("User {Email} found in {Count} tenant(s). Authenticating in {TenantId}.",
            request.Email, allAccessibleIds.Count, authTenant.Id);

        // Authenticate in the first tenant's scope
        using var authScope = _serviceScopeFactory.CreateScope();
        authScope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<AppTenantInfo>(authTenant);

        var identityService = authScope.ServiceProvider.GetRequiredService<IIdentityService>();
        var identityResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, ct);

        if (identityResult is null)
        {
            await AuditLoginFailedSafeAsync(request.Email, clientId, "InvalidCredentials", ip, ct);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var (subject, claims) = identityResult.Value;
        var enrichedClaims = EnrichWithTenantsClaim(claims, allAccessibleIds);

        var securityAudit = authScope.ServiceProvider.GetRequiredService<ISecurityAudit>();
        await securityAudit.LoginSucceededAsync(
            userId: subject,
            userName: enrichedClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
            clientId: clientId!,
            ip: ip,
            userAgent: ua,
            ct: ct);

        var token = await _tokenService.IssueAsync(subject, enrichedClaims, ct);

        await identityService.StoreRefreshTokenAsync(subject, token.RefreshToken, token.RefreshTokenExpiresAt, ct);

        try
        {
            var sessionService = authScope.ServiceProvider.GetRequiredService<ISessionService>();
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
            userName: enrichedClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? request.Email,
            clientId: clientId!,
            tokenFingerprint: fingerprint,
            expiresUtc: token.AccessTokenExpiresAt,
            ct: ct);

        var outboxStore = authScope.ServiceProvider.GetRequiredService<IOutboxStore>();
        await EnqueueTokenGeneratedEventInScopeAsync(request, subject, enrichedClaims, authTenant.Id, clientId, ip, ua, fingerprint, token, outboxStore, ct);

        return token;
    }

    // Scans all active tenants to find where this email exists (used for normal path)
    private async Task<List<string>> GetAllAccessibleTenantIdsAsync(string email, CancellationToken ct)
    {
        var allTenants = (await _tenantStore.GetAllAsync()).Where(t => t.IsActive).ToList();
        var accessibleIds = new List<string>();

        foreach (var tenantInfo in allTenants)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);

            var resolver = scope.ServiceProvider.GetRequiredService<IUserTenantResolver>();
            if (await resolver.UserExistsInTenantAsync(email, ct))
                accessibleIds.Add(tenantInfo.Id!);
        }

        return accessibleIds;
    }

    // Adds the tenants claim to an existing claims collection
    private static List<Claim> EnrichWithTenantsClaim(IEnumerable<Claim> claims, IReadOnlyCollection<string> tenantIds)
    {
        var list = claims.ToList();
        list.AddRange(tenantIds.Select(id => new Claim(ClaimConstants.Tenants, id)));
        return list;
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
