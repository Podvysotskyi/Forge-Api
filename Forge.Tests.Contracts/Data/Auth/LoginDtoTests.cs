using Forge.Contracts.Data.Auth;

namespace Forge.Tests.Contracts.Data.Auth;

public class LoginDtoTests
{
    [Test]
    public void LoginRequestDto_ShouldStoreCredentials()
    {
        var request = new LoginDto
        {
            LoginOrEmail = "jdoe@example.com",
            Password = "secret"
        };

        Assert.Multiple(() =>
        {
            Assert.That(request.LoginOrEmail, Is.EqualTo("jdoe@example.com"));
            Assert.That(request.Password, Is.EqualTo("secret"));
        });
    }
}
