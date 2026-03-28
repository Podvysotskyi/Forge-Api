using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Tests.Persistence;

internal static class RepositoryDbContextTestFactory
{
    public static RepositoryDbContext Create()
    {
        var options = new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new RepositoryDbContext(options);
    }
}
