using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities;

public class EpicTests
{
    [Test]
    public void Epic_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(20);
        var deletedAt = createdAt.AddDays(2);
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Roadmap",
            OrganizationId = Guid.NewGuid(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var epic = new Epic
        {
            Id = Guid.NewGuid(),
            Name = "Authentication",
            ProjectId = project.Id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            Project = project
        };

        Assert.Multiple(() =>
        {
            Assert.That(epic.Name, Is.EqualTo("Authentication"));
            Assert.That(epic.ProjectId, Is.EqualTo(project.Id));
            Assert.That(epic.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(epic.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(epic.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(epic.Project, Is.SameAs(project));
            Assert.That(epic.Tasks, Is.Not.Null);
            Assert.That(epic.Tasks, Is.Empty);
        });
    }
}
