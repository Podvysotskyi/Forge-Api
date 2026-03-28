using Forge.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forge.Tests.Persistence;

public class RepositoryDbContextTests
{
    [Test]
    public void Constructor_ShouldExposeDbSetsForAllEntities()
    {
        var options = new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseInMemoryDatabase(nameof(RepositoryDbContextTests))
            .Options;

        using var dbContext = new RepositoryDbContext(options);

        Assert.Multiple(() =>
        {
            Assert.That(dbContext.Organizations, Is.Not.Null);
            Assert.That(dbContext.OrganizationUsers, Is.Not.Null);
            Assert.That(dbContext.Permissions, Is.Not.Null);
            Assert.That(dbContext.Roles, Is.Not.Null);
            Assert.That(dbContext.Users, Is.Not.Null);
            Assert.That(dbContext.Statuses, Is.Not.Null);
        });
    }
}
