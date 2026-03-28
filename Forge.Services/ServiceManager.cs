using Forge.Domain.Repositories;
using Forge.Services.Abstractions;

namespace Forge.Services;

public class ServiceManager(IRepositoryManager repositoryManager, IPasswordService passwordService) : IServiceManager
{
    private readonly Lazy<IAuthService> _authService = new(() => new AuthService(repositoryManager, passwordService));
    private readonly Lazy<IOrganizationService> _organizationService = new(() => new OrganizationService(repositoryManager));
    private readonly Lazy<IPermissionService> _permissionService = new(() => new PermissionService(repositoryManager));
    private readonly Lazy<IRoleService> _roleService = new(() => new RoleService(repositoryManager));

    public IAuthService AuthService => _authService.Value;
    public IOrganizationService OrganizationService => _organizationService.Value;
    public IPermissionService PermissionService => _permissionService.Value;
    public IRoleService RoleService => _roleService.Value;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return repositoryManager.SaveChangesAsync(cancellationToken);
    }
}
