using Forge.Contracts.Types.Status;

namespace Forge.Tests.Contracts.Enums;

public class OrganizationUserStatusTests
{
    [Test]
    public void OrganizationUserStatus_ShouldExposeExpectedValuesInOrder()
    {
        var values = Enum.GetValues<OrganizationUserStatus>();

        Assert.That(values, Is.EqualTo(new[]
        {
            OrganizationUserStatus.OrganizationActive,
            OrganizationUserStatus.OrganizationPending,
            OrganizationUserStatus.OrganizationBlocked
        }));
    }
}
