namespace Forge.Tests.Persistence.Configurations;

public class RoleConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKey()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Role));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "Id" }));
    }

    [Test]
    public void Configure_ShouldCreateUniqueIndexForName()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Role));

        Assert.That(entityType!.GetIndexes().Any(index =>
                index.Properties.Select(property => property.Name).SequenceEqual(["Name"]) &&
                index.IsUnique),
            Is.True);
    }
}
