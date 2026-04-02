using Microsoft.EntityFrameworkCore;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Tests.Persistence.Configurations;

public class TaskConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndProjectNameIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskEntity));

        Assert.Multiple(() =>
        {
            Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
                Is.EqualTo(new[] { "Id" }));
            Assert.That(entityType.GetIndexes().Any(index =>
                    index.Properties.Select(property => property.Name).SequenceEqual(["ProjectId", "Name"])),
                Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetRequiredRelationshipsToProjectStatusAndPriority()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskEntity))!;

        Assert.Multiple(() =>
        {
            Assert.That(entityType.GetForeignKeys().Any(foreignKey =>
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]) &&
                foreignKey.IsRequired), Is.True);
            Assert.That(entityType.GetForeignKeys().Any(foreignKey =>
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["TaskStatusId"]) &&
                foreignKey.IsRequired), Is.True);
            Assert.That(entityType.GetForeignKeys().Any(foreignKey =>
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["TaskPriorityId"]) &&
                foreignKey.IsRequired), Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetOptionalParentTaskRelationshipWithNoActionDeleteBehavior()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskEntity));
        var foreignKey = entityType!.GetForeignKeys().Single(foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ParentTaskId"]));

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.IsRequired, Is.False);
            Assert.That(foreignKey.DeleteBehavior, Is.EqualTo(DeleteBehavior.NoAction));
        });
    }

    [Test]
    public void Configure_ShouldSetManyToManyRelationsWithEpicsSprintsAndTaskLabels()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskEntity))!;

        Assert.Multiple(() =>
        {
            Assert.That(entityType.GetSkipNavigations().Any(navigation =>
                navigation.Name == "Epics" &&
                navigation.TargetEntityType.ClrType == typeof(TaskForge.Domain.Entities.Epic)), Is.True);
            Assert.That(entityType.GetSkipNavigations().Any(navigation =>
                navigation.Name == "Sprints" &&
                navigation.TargetEntityType.ClrType == typeof(TaskForge.Domain.Entities.Sprint)), Is.True);
            Assert.That(entityType.GetSkipNavigations().Any(navigation =>
                navigation.Name == "TaskLabels" &&
                navigation.TargetEntityType.ClrType == typeof(TaskForge.Domain.Entities.TaskLabel)), Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetOneToManyRelationWithTaskUsers()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskUser));
        var foreignKey = entityType!.GetForeignKeys().Single(foreignKey =>
            foreignKey.PrincipalEntityType.ClrType == typeof(TaskEntity) &&
            foreignKey.Properties.Select(property => property.Name).SequenceEqual(["TaskId"]));

        Assert.That(foreignKey.IsRequired, Is.True);
    }
}
