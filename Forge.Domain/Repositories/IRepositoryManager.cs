namespace Forge.Domain.Repositories;

public interface IRepositoryManager {
    IOrganizationRepository OrganizationRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRepository UserRepository { get; }
    IStatusRepository StatusRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
