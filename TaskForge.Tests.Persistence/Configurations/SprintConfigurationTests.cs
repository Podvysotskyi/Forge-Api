namespace TaskForge.Tests.Persistence.Configurations;

public class SprintConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Sprint));

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
    public void Configure_ShouldSetNameAndDescriptionMaxLength()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Sprint));
        var nameProperty = entityType!.FindProperty("Name");
        var descriptionProperty = entityType.FindProperty("Description");

        Assert.Multiple(() =>
        {
            Assert.That(nameProperty!.GetMaxLength(), Is.EqualTo(100));
            Assert.That(descriptionProperty!.GetMaxLength(), Is.EqualTo(1000));
        });
    }

    [Test]
    public void Configure_ShouldSetRequiredProjectRelationshipAndManyToManyWithTasks()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Sprint));
        var foreignKey = entityType!.GetForeignKeys().Single(foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]));

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.IsRequired, Is.True);
            Assert.That(entityType.GetSkipNavigations().Any(navigation =>
                navigation.Name == "Tasks" &&
                navigation.TargetEntityType.ClrType == typeof(TaskForge.Domain.Entities.Task)), Is.True);
        });
    }
}
