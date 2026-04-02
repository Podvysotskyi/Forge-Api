using TaskForge.Contracts.Types;

namespace TaskForge.Tests.Contracts.Enums;

public class TaskStatusTypeTests
{
    [Test]
    public void EnumValues_ShouldMatchExpectedOrder()
    {
        var values = Enum.GetValues<TaskStatusType>();

        Assert.That(values, Is.EqualTo(new[]
        {
            TaskStatusType.Backlog,
            TaskStatusType.ToDo,
            TaskStatusType.InProgress,
            TaskStatusType.Done,
            TaskStatusType.Cancelled
        }));
    }
}
