namespace FSH.Framework.Shared.Multitenancy;

public interface IUserTenantResolver
{
    Task<bool> UserExistsInTenantAsync(string email, CancellationToken cancellationToken = default);
}
