using TaskForge.Domain.Entities;
using TaskStatusEntity = TaskForge.Domain.Entities.TaskStatus;

namespace TaskForge.Tests.Domain.Entities;

public class TaskStatusTests
{
    [Test]
    public void TaskStatus_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var taskStatus = new TaskStatusEntity
        {
            Id = 2,
            Name = "InProgress"
        };

        Assert.Multiple(() =>
        {
            Assert.That(taskStatus.Id, Is.EqualTo(2));
            Assert.That(taskStatus.Name, Is.EqualTo("InProgress"));
            Assert.That(taskStatus.Tasks, Is.Not.Null);
            Assert.That(taskStatus.Tasks, Is.Empty);
        });
    }
}
