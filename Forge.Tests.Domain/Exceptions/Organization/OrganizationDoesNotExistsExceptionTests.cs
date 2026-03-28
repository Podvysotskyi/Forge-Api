using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Organization;

namespace Forge.Tests.Domain.Exceptions.Organization;

public class OrganizationDoesNotExistsExceptionTests
{
    [Test]
    public void OrganizationDoesNotExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new OrganizationDoesNotExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("Organization does not exists"));
        });
    }
}
