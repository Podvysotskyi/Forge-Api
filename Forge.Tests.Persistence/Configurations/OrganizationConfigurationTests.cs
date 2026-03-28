using Microsoft.EntityFrameworkCore;

namespace Forge.Tests.Persistence.Configurations;

public class OrganizationConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKey()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Organization));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "Id" }));
    }

    [Test]
    public void Configure_ShouldCreateUniqueIndexForName()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Organization));

        Assert.That(entityType!.GetIndexes().Any(index =>
                index.Properties.Select(property => property.Name).SequenceEqual(["Name"]) &&
                index.IsUnique),
            Is.True);
    }

    [Test]
    public void Configure_ShouldSetOptionalOneToOneRelationshipToUser()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.Organization));
        var foreignKey = entityType!.GetForeignKeys().Single();

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.Properties.Select(property => property.Name), Is.EqualTo(new[] { "UserId" }));
            Assert.That(foreignKey.PrincipalEntityType.ClrType, Is.EqualTo(typeof(Forge.Domain.Entities.User)));
            Assert.That(foreignKey.IsUnique, Is.True);
            Assert.That(foreignKey.IsRequired, Is.False);
            Assert.That(foreignKey.DeleteBehavior, Is.EqualTo(DeleteBehavior.SetNull));
        });
    }
}
