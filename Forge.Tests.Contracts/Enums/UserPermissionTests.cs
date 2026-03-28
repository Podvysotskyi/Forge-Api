using Forge.Contracts.Types.Permission;

namespace Forge.Tests.Contracts.Enums;

public class UserPermissionTests
{
    [Test]
    public void UserPermission_ShouldExposeExpectedValuesInOrder()
    {
        var values = Enum.GetValues<UserPermission>();

        Assert.That(values, Is.EqualTo(new[]
        {
            UserPermission.UserManagement,
            UserPermission.ViewRoles,
            UserPermission.ViewPermissions
        }));
    }
}
