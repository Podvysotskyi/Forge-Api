using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Status;

namespace Forge.Tests.Domain.Exceptions.Status;

public class StatusDoesNotExistsExceptionTests
{
    [Test]
    public void StatusDoesNotExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new StatusDoesNotExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("status does not exists"));
        });
    }
}
