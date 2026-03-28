using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Organization;

namespace Forge.Tests.Domain.Exceptions.Organization;

public class OrganizationAlreadyExistsExceptionTests
{
    [Test]
    public void OrganizationAlreadyExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new OrganizationAlreadyExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("Organization already exists"));
        });
    }
}
