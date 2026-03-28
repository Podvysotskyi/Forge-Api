namespace Forge.Tests.Persistence.Configurations;

public class UserConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKey()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.User));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "Id" }));
    }

    [Test]
    public void Configure_ShouldCreateUniqueIndexForLogin()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.User));

        Assert.That(entityType!.GetIndexes().Any(index =>
                index.Properties.Select(property => property.Name).SequenceEqual(["Login"]) &&
                index.IsUnique),
            Is.True);
    }

    [Test]
    public void Configure_ShouldCreateUniqueIndexForEmail()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.User));

        Assert.That(entityType!.GetIndexes().Any(index =>
                index.Properties.Select(property => property.Name).SequenceEqual(["Email"]) &&
                index.IsUnique),
            Is.True);
    }

    [Test]
    public void Configure_ShouldSetRequiredRelationshipToStatus()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.User));
        var foreignKey = entityType!.GetForeignKeys().Single();

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.Properties.Select(property => property.Name), Is.EqualTo(new[] { "StatusId" }));
            Assert.That(foreignKey.PrincipalEntityType.ClrType, Is.EqualTo(typeof(Forge.Domain.Entities.Status)));
            Assert.That(foreignKey.IsRequired, Is.True);
        });
    }
}
