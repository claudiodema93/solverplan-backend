using System.Security.Claims;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Multitenancy.Contracts;
using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenants;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryHandler(
    ITenantService tenantService,
    IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTenantsQuery, PagedResponse<TenantDto>>
{
    public async ValueTask<PagedResponse<TenantDto>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var user = httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException();

        var activeTenant = user.FindFirst(ClaimConstants.ActiveTenant)?.Value;
        var isRootAdmin = string.Equals(activeTenant, MultitenancyConstants.Root.Id, StringComparison.OrdinalIgnoreCase)
            && user.IsInRole("Admin");

        if (isRootAdmin)
        {
            return await tenantService.GetAllAsync(query, null, cancellationToken).ConfigureAwait(false);
        }

        var email = user.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedException();

        var accessibleTenantIds = await tenantService.GetAccessibleTenantIdsAsync(email, cancellationToken).ConfigureAwait(false);
        return await tenantService.GetAllAsync(query, accessibleTenantIds, cancellationToken).ConfigureAwait(false);
    }
}
