using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities;

public class SprintTests
{
    [Test]
    public void Sprint_ShouldInitializeScalarPropertiesAndRelationshipCollections()
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

        var sprint = new Sprint
        {
            Id = Guid.NewGuid(),
            Name = "Sprint 1",
            Description = "Initial sprint",
            ProjectId = project.Id,
            StartDate = DateOnly.FromDateTime(createdAt.Date),
            EndDate = DateOnly.FromDateTime(createdAt.Date.AddDays(14)),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            Project = project
        };

        Assert.Multiple(() =>
        {
            Assert.That(sprint.Name, Is.EqualTo("Sprint 1"));
            Assert.That(sprint.Description, Is.EqualTo("Initial sprint"));
            Assert.That(sprint.ProjectId, Is.EqualTo(project.Id));
            Assert.That(sprint.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(sprint.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(sprint.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(sprint.Project, Is.SameAs(project));
            Assert.That(sprint.Tasks, Is.Not.Null);
            Assert.That(sprint.Tasks, Is.Empty);
        });
    }
}
