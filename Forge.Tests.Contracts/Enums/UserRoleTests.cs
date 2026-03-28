using Forge.Contracts.Types.Role;

namespace Forge.Tests.Contracts.Enums;

public class UserRoleTests
{
    [Test]
    public void RoleType_ShouldExposeExpectedValuesInOrder()
    {
        var values = Enum.GetValues<UserRole>();

        Assert.That(values, Is.EqualTo(new[]
        {
            UserRole.Test,
            UserRole.Admin,
            UserRole.Developer,
            UserRole.User
        }));
    }
}
