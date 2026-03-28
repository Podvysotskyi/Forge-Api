namespace Forge.Tests.Persistence.Configurations;

public class OrganizationUserConfigurationTests
{
    [Test]
    public void Configure_ShouldSetCompositePrimaryKey()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "OrganizationId", "UserId" }));
    }

    [Test]
    public void Configure_ShouldSetForeignKeysForRelationships()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));
        var foreignKeys = entityType!.GetForeignKeys().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(foreignKeys.Any(foreignKey =>
                    foreignKey.PrincipalEntityType.ClrType == typeof(Forge.Domain.Entities.Organization) &&
                    foreignKey.Properties.Select(property => property.Name).SequenceEqual(["OrganizationId"])),
                Is.True);
              Assert.That(foreignKeys.Any(foreignKey =>
                      foreignKey.PrincipalEntityType.ClrType == typeof(Forge.Domain.Entities.User) &&
                      foreignKey.Properties.Select(property => property.Name).SequenceEqual(["UserId"])),
                  Is.True);
              Assert.That(foreignKeys.Any(foreignKey =>
                      foreignKey.PrincipalEntityType.ClrType == typeof(Forge.Domain.Entities.Status) &&
                      foreignKey.Properties.Select(property => property.Name).SequenceEqual(["StatusId"])),
                  Is.True);
              Assert.That(foreignKeys.Any(foreignKey =>
                      foreignKey.PrincipalEntityType.ClrType == typeof(Forge.Domain.Entities.Role) &&
                      foreignKey.Properties.Select(property => property.Name).SequenceEqual(["RoleId"])),
                  Is.True);
          });
    }

    [Test]
    public void Configure_ShouldSetManyToManyRelationshipWithPermissions()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));
        var skipNavigation = entityType!.FindSkipNavigation(nameof(Forge.Domain.Entities.OrganizationUser.Permissions));

        Assert.Multiple(() =>
        {
            Assert.That(skipNavigation, Is.Not.Null);
            Assert.That(skipNavigation!.TargetEntityType.ClrType, Is.EqualTo(typeof(Forge.Domain.Entities.Permission)));
            Assert.That(skipNavigation.Inverse!.Name, Is.EqualTo(nameof(Forge.Domain.Entities.Permission.OrganizationUsers)));
        });
    }

    [Test]
    public void Configure_ShouldRequireCreatedAtAndUpdatedAt()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));
        var createdAtProperty = entityType!.FindProperty(nameof(Forge.Domain.Entities.OrganizationUser.CreatedAt));
        var updatedAtProperty = entityType.FindProperty(nameof(Forge.Domain.Entities.OrganizationUser.UpdatedAt));

        Assert.Multiple(() =>
        {
            Assert.That(createdAtProperty, Is.Not.Null);
            Assert.That(createdAtProperty!.IsNullable, Is.False);
            Assert.That(updatedAtProperty, Is.Not.Null);
            Assert.That(updatedAtProperty!.IsNullable, Is.False);
        });
    }

    [Test]
    public void Configure_ShouldRequireStatusId()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(Forge.Domain.Entities.OrganizationUser));
        var statusIdProperty = entityType!.FindProperty(nameof(Forge.Domain.Entities.OrganizationUser.StatusId));

        Assert.Multiple(() =>
        {
            Assert.That(statusIdProperty, Is.Not.Null);
            Assert.That(statusIdProperty!.IsNullable, Is.False);
        });
    }
}
