using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.User;

namespace Forge.Tests.Domain.Exceptions.User;

public class UserAlreadyExistsExceptionTests
{
    [Test]
    public void UserAlreadyExistsException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new UserAlreadyExistsException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("User already exists"));
        });
    }
}
