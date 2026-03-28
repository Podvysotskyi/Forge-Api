using Forge.Contracts.Types.Permission;
using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Database.Seeders.Permission;

public static class UserPermissionSeeder
{
    public static async Task SeedAsync(
        RepositoryDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var userPermissionNames = Enum.GetNames<UserPermission>().AsEnumerable();
        var existingPermissions = await dbContext.Permissions
            .Where(permission => userPermissionNames.Contains(permission.Name))
            .ToDictionaryAsync(permission => permission.Name, cancellationToken);

        foreach (var userPermissionName in userPermissionNames)
        {
            if (existingPermissions.TryGetValue(userPermissionName, out var existingPermission))
            {
                existingPermission.DeletedAt = null;
                continue;
            }

            await dbContext.Permissions.AddAsync(new Domain.Entities.Permission
            {
                Name = userPermissionName,
                DeletedAt = null
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
