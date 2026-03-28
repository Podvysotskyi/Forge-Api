using Forge.Contracts.Types.Role;
using Forge.Domain.Entities;

namespace Forge.Domain.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> UserRoles(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> OrganizationUserRoles(CancellationToken cancellationToken = default);
    Task<Role> GetById(int id, CancellationToken cancellationToken = default);
    Task<Role> Find(UserRole type, CancellationToken cancellationToken = default);
    Task<Role> Find(OrganizationUserRole type, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> LoadUsers(Role role, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> LoadPermissions(Role role, CancellationToken cancellationToken = default);
}
