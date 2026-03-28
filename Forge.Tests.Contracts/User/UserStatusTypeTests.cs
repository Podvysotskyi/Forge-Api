using Forge.Contracts.Types.Status;

namespace Forge.Tests.Contracts.User;

public class UserStatusTests
{
    [Test]
    public void UserStatus_ShouldExposeExpectedValuesInOrder()
    {
        var values = Enum.GetValues<UserStatus>();

        Assert.That(values, Is.EqualTo(new[]
        {
            UserStatus.Active,
            UserStatus.Blocked,
            UserStatus.Pending
        }));
    }
}
