using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace FSH.Modules.Identity.Services;

public sealed class UserTenantResolver(UserManager<FshUser> userManager) : IUserTenantResolver
{
    public async Task<bool> UserExistsInTenantAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null && user.IsActive;
    }
}
