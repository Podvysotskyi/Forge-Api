using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.Auth;
using Forge.Domain.Exceptions.User;

namespace Forge.Tests.Domain.Exceptions.User;

public class UserDoesNotExistsExceptionTests
{
    [Test]
    public void UserDoesNotExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new UserDoesNotExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("User does not exists"));
        });
    }
}
