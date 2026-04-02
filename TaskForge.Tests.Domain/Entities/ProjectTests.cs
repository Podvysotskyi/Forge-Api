using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities;

public class ProjectTests
{
    [Test]
    public void Project_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddHours(2);
        var deletedAt = createdAt.AddDays(7);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Roadmap",
            OrganizationId = Guid.NewGuid(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt
        };

        Assert.Multiple(() =>
        {
            Assert.That(project.Name, Is.EqualTo("Roadmap"));
            Assert.That(project.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(project.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(project.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(project.TaskLabels, Is.Not.Null);
            Assert.That(project.TaskLabels, Is.Empty);
            Assert.That(project.Epics, Is.Not.Null);
            Assert.That(project.Epics, Is.Empty);
            Assert.That(project.Sprints, Is.Not.Null);
            Assert.That(project.Sprints, Is.Empty);
            Assert.That(project.Tasks, Is.Not.Null);
            Assert.That(project.Tasks, Is.Empty);
        });
    }
}
