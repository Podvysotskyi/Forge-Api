using Forge.Contracts.Data.Organization;

namespace Forge.Tests.Contracts.Organization;

public class CreateOrganizationDtoTests
{
    [Test]
    public void CreateOrganizationDto_ShouldStoreName()
    {
        var dto = new CreateOrganizationDto
        {
            Name = "Acme"
        };

        Assert.That(dto.Name, Is.EqualTo("Acme"));
    }
}
