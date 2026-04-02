using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities;

public class TaskLabelTests
{
    [Test]
    public void TaskLabel_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(30);
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Roadmap",
            OrganizationId = Guid.NewGuid(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var taskLabel = new TaskLabel
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Name = "Backend",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Project = project
        };

        Assert.Multiple(() =>
        {
            Assert.That(taskLabel.ProjectId, Is.EqualTo(project.Id));
            Assert.That(taskLabel.Name, Is.EqualTo("Backend"));
            Assert.That(taskLabel.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(taskLabel.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(taskLabel.Project, Is.SameAs(project));
            Assert.That(taskLabel.Tasks, Is.Not.Null);
            Assert.That(taskLabel.Tasks, Is.Empty);
        });
    }
}
