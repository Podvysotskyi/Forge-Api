using Microsoft.EntityFrameworkCore;

namespace TaskForge.Tests.Persistence;

internal static class TaskForgeDbContextTestFactory
{
    public static TaskForgeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TaskForgeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new TaskForgeDbContext(options);
    }
}
