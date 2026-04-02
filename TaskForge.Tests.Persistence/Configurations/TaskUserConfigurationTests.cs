namespace TaskForge.Tests.Persistence.Configurations;

public class TaskUserConfigurationTests
{
    [Test]
    public void Configure_ShouldSetCompositePrimaryKey()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskUser));

        Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
            Is.EqualTo(new[] { "TaskId", "UserId" }));
    }

    [Test]
    public void Configure_ShouldSetRequiredRelationshipToTask()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.TaskUser));
        var foreignKey = entityType!.GetForeignKeys().Single();

        Assert.Multiple(() =>
        {
            Assert.That(foreignKey.Properties.Select(property => property.Name), Is.EqualTo(new[] { "TaskId" }));
            Assert.That(foreignKey.PrincipalEntityType.ClrType, Is.EqualTo(typeof(TaskForge.Domain.Entities.Task)));
            Assert.That(foreignKey.IsRequired, Is.True);
        });
    }
}
