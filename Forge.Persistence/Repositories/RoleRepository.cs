using Forge.Contracts.Types.Role;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Role;
using Forge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence.Repositories;

public class RoleRepository(RepositoryDbContext dbContext) : Repository<Role>(dbContext), IRoleRepository
{
    public async Task<List<Role>> UserRoles(CancellationToken cancellationToken = default)
    {
        var names = Enum.GetNames<UserRole>().AsEnumerable();

        return await Entities
            .AsNoTracking()
            .Where(role => role.DeletedAt == null && names.Contains(role.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> OrganizationUserRoles(CancellationToken cancellationToken = default)
    {
        var names = Enum.GetNames<OrganizationUserRole>().AsEnumerable();

        return await Entities
            .Where(role => role.DeletedAt == null && names.Contains(role.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<Role> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(role => role.Id == id && role.DeletedAt == null, cancellationToken)
            ?? throw new RoleDoesNotExistsException();
    }

    public async Task<Role> Find(UserRole type, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(role => role.Name == type.ToString() && role.DeletedAt == null, cancellationToken)
            ?? throw new RoleDoesNotExistsException();
    }

    public async Task<Role> Find(OrganizationUserRole type, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(role => role.Name == type.ToString() && role.DeletedAt == null, cancellationToken)
            ?? throw new RoleDoesNotExistsException();
    }

    public async Task<IEnumerable<User>> LoadUsers(Role role, CancellationToken cancellationToken = default)
    {
        if (!Entry(role).Collection(currentRole => currentRole.Users).IsLoaded)
        {
            await Entry(role)
                .Collection(currentRole => currentRole.Users)
                .Query()
                .Where(user => user.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return role.Users;
    }

    public async Task<IEnumerable<Permission>> LoadPermissions(Role role, CancellationToken cancellationToken = default)
    {
        if (!Entry(role).Collection(currentRole => currentRole.Permissions).IsLoaded)
        {
            await Entry(role)
                .Collection(currentRole => currentRole.Permissions)
                .Query()
                .Where(permission => permission.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return role.Permissions;
    }
}
