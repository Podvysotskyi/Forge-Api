using TaskForge.Domain.Entities;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Tests.Domain.Entities;

public class TaskUserTests
{
    [Test]
    public void TaskUser_ShouldInitializeScalarPropertiesAndRelationships()
    {
        var createdAt = DateTime.UtcNow;
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Name = "Implement API",
            ProjectId = Guid.NewGuid(),
            TaskStatusId = 1,
            TaskPriorityId = 2,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddMinutes(5)
        };

        var taskUser = new TaskUser
        {
            TaskId = task.Id,
            UserId = Guid.NewGuid(),
            CreatedAt = createdAt,
            Task = task
        };

        Assert.Multiple(() =>
        {
            Assert.That(taskUser.TaskId, Is.EqualTo(task.Id));
            Assert.That(taskUser.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(taskUser.Task, Is.SameAs(task));
        });
    }
}
