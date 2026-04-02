using Microsoft.EntityFrameworkCore;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Tests.Persistence.Configurations;

public class TaskLabelConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskLabel));

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
    public void Configure_ShouldSetProjectRelationshipWithNoActionDeleteBehavior()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskLabel));
        var foreignKey = entityType!.GetForeignKeys().Single(foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]));

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.IsRequired, Is.True);
            Assert.That(foreignKey.DeleteBehavior, Is.EqualTo(DeleteBehavior.NoAction));
        });
    }

    [Test]
    public void Configure_ShouldSetManyToManyRelationshipWithTasks()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskLabel))!;

        Assert.That(entityType.GetSkipNavigations().Any(navigation =>
            navigation.Name == "Tasks" &&
            navigation.TargetEntityType.ClrType == typeof(TaskEntity)), Is.True);
    }
}
