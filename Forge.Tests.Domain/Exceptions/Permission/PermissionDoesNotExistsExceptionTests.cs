using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Permission;

namespace Forge.Tests.Domain.Exceptions.Permission;

public class PermissionDoesNotExistsExceptionTests
{
    [Test]
    public void PermissionDoesNotExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new PermissionDoesNotExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("Permission does not exists"));
        });
    }
}
