using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Auth;

namespace Forge.Tests.Domain.Exceptions.Auth;

public class InvalidCredentialsExceptionTests
{
    [Test]
    public void InvalidCredentialsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new InvalidCredentialsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("Invalid credentials"));
        });
    }
}
