namespace TaskForge.Tests.Persistence.Configurations;

public class EpicConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Epic));

        Assert.Multiple(() =>
        {
            Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
                Is.EqualTo(new[] { "Id" }));
            Assert.That(entityType.GetIndexes().Any(index =>
                    index.Properties.Select(property => property.Name).SequenceEqual(["ProjectId", "Name"]) &&
                    index.IsUnique),
                Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetRequiredProjectRelationship()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Epic));
        var foreignKey = entityType!.GetForeignKeys().Single(foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]));

        Assert.That(foreignKey.IsRequired, Is.True);
    }

    [Test]
    public void Configure_ShouldSetManyToManyRelationshipWithTasks()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Epic))!;

        Assert.That(entityType.GetSkipNavigations().Any(navigation =>
            navigation.Name == "Tasks" &&
            navigation.TargetEntityType.ClrType == typeof(TaskForge.Domain.Entities.Task)), Is.True);
    }
}
