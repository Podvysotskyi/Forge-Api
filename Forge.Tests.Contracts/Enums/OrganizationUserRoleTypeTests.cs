using Forge.Contracts.Types.Role;

namespace Forge.Tests.Contracts.Enums;

public class OrganizationUserRoleTests
{
    [Test]
    public void OrganizationUserRole_ShouldExposeExpectedValuesInOrder()
    {
        var values = Enum.GetValues<OrganizationUserRole>();

        Assert.That(values, Is.EqualTo(new[]
        {
            OrganizationUserRole.OrganizationOwner,
            OrganizationUserRole.OrganizationManager,
            OrganizationUserRole.OrganizationUser
        }));
    }
}
