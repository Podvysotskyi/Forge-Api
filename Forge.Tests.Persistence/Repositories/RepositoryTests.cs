using Forge.Domain.Entities;
using Forge.Persistence;
using Forge.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Forge.Tests.Persistence.Repositories;

public class RepositoryTests
{
    [Test]
    public async Task Entities_ShouldReturnDbSetForEntityType()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.Add(new Permission { Id = 1, Name = "Users.Read" });
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository(dbContext);

        var result = await repository.EntitiesQuery.ToListAsync();

        Assert.That(result.Select(permission => permission.Name), Is.EqualTo(["Users.Read"]));
    }

    [Test]
    public async Task Entry_ShouldReturnEntityEntryForProvidedEntity()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var permission = new Permission { Id = 1, Name = "Users.Read" };
        dbContext.Permissions.Add(permission);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository(dbContext);

        var result = repository.GetEntry(permission);

        Assert.Multiple(() =>
        {
            Assert.That(result.Entity, Is.SameAs(permission));
            Assert.That(result.State, Is.EqualTo(EntityState.Unchanged));
        });
    }

    private sealed class TestRepository(RepositoryDbContext dbContext) : Repository<Permission>(dbContext)
    {
        public IQueryable<Permission> EntitiesQuery => Entities;

        public EntityEntry<Permission> GetEntry(Permission permission) => Entry(permission);
    }
}
