using Forge.Contracts.Types.Status;
using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Database.Seeders.Status;

public static class OrganizationUserStatusSeeder
{
    public static async Task SeedAsync(
        RepositoryDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var organizationUserStatusNames = Enum.GetNames<OrganizationUserStatus>().AsEnumerable();
        var existingStatuses = await dbContext.Statuses
            .Where(status => organizationUserStatusNames.Contains(status.Name))
            .ToDictionaryAsync(status => status.Name, cancellationToken);

        foreach (var organizationUserStatusName in organizationUserStatusNames)
        {
            if (existingStatuses.TryGetValue(organizationUserStatusName, out var existingStatus))
            {
                existingStatus.DeletedAt = null;
                continue;
            }

            await dbContext.Statuses.AddAsync(new Domain.Entities.Status
            {
                Name = organizationUserStatusName,
                DeletedAt = null
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
