using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Permission;
using Forge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence.Repositories;

public class PermissionRepository(RepositoryDbContext dbContext) : Repository<Permission>(dbContext), IPermissionRepository
{
    public async Task<List<Permission>> GetAll(CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(permission => permission.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(permission => permission.Id == id && permission.DeletedAt == null, cancellationToken)
            ?? throw new PermissionDoesNotExistsException();
    }

    public async Task<Permission> GetByName(string name, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(permission => permission.Name == name && permission.DeletedAt == null, cancellationToken)
            ?? throw new PermissionDoesNotExistsException();
    }

    public async Task<IEnumerable<Role>> LoadRoles(Permission permission, CancellationToken cancellationToken = default)
    {
        if (!Entry(permission).Collection(currentPermission => currentPermission.Roles).IsLoaded)
        {
            await Entry(permission)
                .Collection(currentPermission => currentPermission.Roles)
                .Query()
                .Where(role => role.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return permission.Roles.ToList();
    }
}
