using Microsoft.EntityFrameworkCore;
using TaskStatusEntity = TaskForge.Domain.Entities.TaskStatus;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Tests.Persistence.Configurations;

public class TaskStatusConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueNameIndex()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskStatusEntity));

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
            fk.PrincipalEntityType.ClrType == typeof(TaskStatusEntity) &&
            fk.Properties.Select(property => property.Name).SequenceEqual(["TaskStatusId"]));

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.IsRequired, Is.True);
            Assert.That(foreignKey.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
        });
    }
}
