using Forge.Contracts.Types.Role;
using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Database.Seeders.Role;

public static class UserRoleSeeder
{
    public static async Task SeedAsync(
        RepositoryDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var userRoleNames = Enum.GetNames<UserRole>().AsEnumerable();
        var existingRoles = await dbContext.Roles
            .Where(role => userRoleNames.Contains(role.Name))
            .ToDictionaryAsync(role => role.Name, cancellationToken);

        foreach (var userRoleName in userRoleNames)
        {
            if (existingRoles.TryGetValue(userRoleName, out var existingRole))
            {
                existingRole.DeletedAt = null;
                continue;
            }

            await dbContext.Roles.AddAsync(new Domain.Entities.Role
            {
                Name = userRoleName,
                DeletedAt = null
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
