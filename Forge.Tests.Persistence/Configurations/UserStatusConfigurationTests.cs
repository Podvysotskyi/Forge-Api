namespace Forge.Tests.Persistence.Configurations;

public class UserStatusConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKey()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Status));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "Id" }));
    }

    [Test]
    public void Configure_ShouldCreateUniqueIndexForName()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Status));

        Assert.That(entityType!.GetIndexes().Any(index =>
                index.Properties.Select(property => property.Name).SequenceEqual(["Name"]) &&
                index.IsUnique),
            Is.True);
    }

    [Test]
    public void Configure_ShouldSetForeignKeyForOrganizationUsers()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));
        var foreignKey = entityType!.GetForeignKeys().SingleOrDefault(currentForeignKey =>
            currentForeignKey.PrincipalEntityType.ClrType == typeof(Forge.Domain.Entities.Status) &&
            currentForeignKey.Properties.Select(property => property.Name).SequenceEqual(["StatusId"]));

        Assert.That(foreignKey, Is.Not.Null);
    }
}
