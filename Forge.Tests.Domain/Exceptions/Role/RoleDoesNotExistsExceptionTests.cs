using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Role;

namespace Forge.Tests.Domain.Exceptions.Role;

public class RoleDoesNotExistsExceptionTests
{
    [Test]
    public void RoleDoesNotExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new RoleDoesNotExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("Role does not exists"));
        });
    }
}
