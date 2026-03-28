using Forge.Contracts.Types.Status;
using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Database.Seeders.Status;

public static class UserStatusSeeder
{
    public static async Task SeedAsync(
        RepositoryDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var userStatusNames = Enum.GetNames<UserStatus>().AsEnumerable();
        var existingStatuses = await dbContext.Statuses
            .Where(status => userStatusNames.Contains(status.Name))
            .ToDictionaryAsync(status => status.Name, cancellationToken);

        foreach (var userStatusName in userStatusNames)
        {
            if (existingStatuses.TryGetValue(userStatusName, out var existingStatus))
            {
                existingStatus.DeletedAt = null;
                continue;
            }

            await dbContext.Statuses.AddAsync(new Domain.Entities.Status
            {
                Name = userStatusName,
                DeletedAt = null
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
