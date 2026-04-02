using TaskForge.Contracts.Types;

namespace TaskForge.Tests.Contracts.Enums;

public class TaskPriorityTypeTests
{
    [Test]
    public void EnumValues_ShouldMatchExpectedOrder()
    {
        var values = Enum.GetValues<TaskPriorityType>();

        Assert.That(values, Is.EqualTo(new[]
        {
            TaskPriorityType.Urgent,
            TaskPriorityType.High,
            TaskPriorityType.Medium,
            TaskPriorityType.Low,
            TaskPriorityType.Default
        }));
    }
}
