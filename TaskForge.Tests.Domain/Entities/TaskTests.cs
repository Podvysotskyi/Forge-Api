using TaskForge.Domain.Entities;
using TaskEntity = TaskForge.Domain.Entities.Task;
using TaskStatusEntity = TaskForge.Domain.Entities.TaskStatus;

namespace TaskForge.Tests.Domain.Entities;

public class TaskTests
{
    [Test]
    public void Task_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(5);
        var deletedAt = createdAt.AddDays(1);
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Roadmap",
            OrganizationId = Guid.NewGuid(),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        var taskStatus = new TaskStatusEntity
        {
            Id = 1,
            Name = "ToDo"
        };
        var taskPriority = new TaskPriority
        {
            Id = 2,
            Name = "High"
        };
        var parentTask = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            ProjectId = project.Id,
            TaskStatusId = taskStatus.Id,
            TaskPriorityId = taskPriority.Id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Name = "Child",
            Description = "Implement endpoint",
            ProjectId = project.Id,
            ParentTaskId = parentTask.Id,
            CreatedByUserId = Guid.NewGuid(),
            AssignedToUserId = Guid.NewGuid(),
            TaskStatusId = taskStatus.Id,
            TaskPriorityId = taskPriority.Id,
            StartDate = DateOnly.FromDateTime(createdAt.Date),
            DueDate = DateOnly.FromDateTime(createdAt.Date.AddDays(3)),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            ParentTask = parentTask,
            Project = project,
            TaskStatus = taskStatus,
            TaskPriority = taskPriority
        };

        Assert.Multiple(() =>
        {
            Assert.That(task.Name, Is.EqualTo("Child"));
            Assert.That(task.Description, Is.EqualTo("Implement endpoint"));
            Assert.That(task.ProjectId, Is.EqualTo(project.Id));
            Assert.That(task.ParentTaskId, Is.EqualTo(parentTask.Id));
            Assert.That(task.TaskStatusId, Is.EqualTo(taskStatus.Id));
            Assert.That(task.TaskPriorityId, Is.EqualTo(taskPriority.Id));
            Assert.That(task.StartDate, Is.EqualTo(DateOnly.FromDateTime(createdAt.Date)));
            Assert.That(task.DueDate, Is.EqualTo(DateOnly.FromDateTime(createdAt.Date.AddDays(3))));
            Assert.That(task.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(task.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(task.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(task.ParentTask, Is.SameAs(parentTask));
            Assert.That(task.Project, Is.SameAs(project));
            Assert.That(task.TaskStatus, Is.SameAs(taskStatus));
            Assert.That(task.TaskPriority, Is.SameAs(taskPriority));
            Assert.That(task.Epics, Is.Not.Null);
            Assert.That(task.Epics, Is.Empty);
            Assert.That(task.Sprints, Is.Not.Null);
            Assert.That(task.Sprints, Is.Empty);
            Assert.That(task.Tasks, Is.Not.Null);
            Assert.That(task.Tasks, Is.Empty);
            Assert.That(task.TaskUsers, Is.Not.Null);
            Assert.That(task.TaskUsers, Is.Empty);
            Assert.That(task.TaskLabels, Is.Not.Null);
            Assert.That(task.TaskLabels, Is.Empty);
        });
    }
}
