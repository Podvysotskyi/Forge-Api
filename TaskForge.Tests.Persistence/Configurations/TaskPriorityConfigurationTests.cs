using Microsoft.EntityFrameworkCore;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Tests.Persistence.Configurations;

public class TaskPriorityConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueNameIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskPriority));

        Assert.Multiple(() =>
        {
            Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
                Is.EqualTo(new[] { "Id" }));
            Assert.That(entityType.GetIndexes().Any(index =>
                    index.Properties.Select(property => property.Name).SequenceEqual(["Name"]) &&
                    index.IsUnique),
                Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetRequiredRelationshipToTask()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskEntity));
        var foreignKey = entityType!.GetForeignKeys().Single(fk =>
            fk.PrincipalEntityType.ClrType == typeof(TaskForge.Domain.Entities.TaskPriority) &&
            fk.Properties.Select(property => property.Name).SequenceEqual(["TaskPriorityId"]));

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.IsRequired, Is.True);
            Assert.That(foreignKey.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        });
    }
}
