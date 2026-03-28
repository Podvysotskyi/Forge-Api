using Forge.Domain.Exceptions;
using Forge.Domain.Exceptions.User;

namespace Forge.Tests.Domain.Exceptions.User;

public class UserStatusIsNotLoadedExceptionTests
{
    [Test]
    public void UserStatusIsNotLoadedException_ShouldExposeExpectedMessageAndBaseType()
    {
        var exception = new UserStatusIsNotLoadedException();

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.InstanceOf<DomainException>());
            Assert.That(exception.Message, Is.EqualTo("User status is not loaded"));
        });
    }
}
