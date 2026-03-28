namespace Forge.Services.Abstractions;

public interface IServiceManager
{
    IAuthService AuthService { get; }
    IOrganizationService OrganizationService { get; }
    IPermissionService PermissionService { get; }
    IRoleService RoleService { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}