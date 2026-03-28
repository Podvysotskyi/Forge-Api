using Forge.Contracts.Types.Role;
using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Database.Seeders.Role;

public static class OrganizationUserRoleSeeder
{
    public static async Task SeedAsync(
        RepositoryDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var organizationUserRoleNames = Enum.GetNames<OrganizationUserRole>().AsEnumerable();
        var existingRoles = await dbContext.Roles
            .Where(role => organizationUserRoleNames.Contains(role.Name))
            .ToDictionaryAsync(role => role.Name, cancellationToken);

        foreach (var organizationUserRoleName in organizationUserRoleNames)
        {
            if (existingRoles.TryGetValue(organizationUserRoleName, out var existingRole))
            {
                existingRole.DeletedAt = null;
                continue;
            }

            await dbContext.Roles.AddAsync(new Domain.Entities.Role
            {
                Name = organizationUserRoleName,
                DeletedAt = null
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
