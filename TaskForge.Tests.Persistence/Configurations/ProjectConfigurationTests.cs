namespace TaskForge.Tests.Persistence.Configurations;

public class ProjectConfigurationTests
{
    [Test]
    public void Configure_ShouldSetPrimaryKeyAndUniqueNameIndexPerOrganization()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var entityType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Project));

        Assert.Multiple(() =>
        {
            Assert.That(entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name),
                Is.EqualTo(new[] { "Id" }));
            Assert.That(entityType.GetIndexes().Any(index =>
                    index.Properties.Select(property => property.Name).SequenceEqual(["OrganizationId", "Name"]) &&
                    index.IsUnique),
                Is.True);
        });
    }

    [Test]
    public void Configure_ShouldSetRequiredRelationshipsToTaskLabelEpicSprintAndTask()
    {
        using var dbContext = TaskForgeDbContextTestFactory.Create();
        var projectType = dbContext.Model.FindEntityType(typeof(TaskForge.Domain.Entities.Project))!;

        var foreignKeys = dbContext.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey => foreignKey.PrincipalEntityType == projectType)
            .ToList();

        Assert.Multiple(() =>
        {
            Assert.That(foreignKeys.Any(foreignKey =>
                foreignKey.DeclaringEntityType.ClrType == typeof(TaskForge.Domain.Entities.TaskLabel) &&
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]) &&
                foreignKey.IsRequired), Is.True);
            Assert.That(foreignKeys.Any(foreignKey =>
                foreignKey.DeclaringEntityType.ClrType == typeof(TaskForge.Domain.Entities.Epic) &&
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]) &&
                foreignKey.IsRequired), Is.True);
            Assert.That(foreignKeys.Any(foreignKey =>
                foreignKey.DeclaringEntityType.ClrType == typeof(TaskForge.Domain.Entities.Sprint) &&
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]) &&
                foreignKey.IsRequired), Is.True);
            Assert.That(foreignKeys.Any(foreignKey =>
                foreignKey.DeclaringEntityType.ClrType == typeof(TaskForge.Domain.Entities.Task) &&
                foreignKey.Properties.Select(property => property.Name).SequenceEqual(["ProjectId"]) &&
                foreignKey.IsRequired), Is.True);
        });
    }
}
