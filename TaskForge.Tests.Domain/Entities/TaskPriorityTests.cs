using TaskForge.Domain.Entities;

namespace TaskForge.Tests.Domain.Entities;

public class TaskPriorityTests
{
    [Test]
    public void TaskPriority_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var taskPriority = new TaskPriority
        {
            Id = 1,
            Name = "High"
        };

        Assert.Multiple(() =>
        {
            Assert.That(taskPriority.Id, Is.EqualTo(1));
            Assert.That(taskPriority.Name, Is.EqualTo("High"));
            Assert.That(taskPriority.Tasks, Is.Not.Null);
            Assert.That(taskPriority.Tasks, Is.Empty);
        });
    }
}
