using Forge.Contracts.Data.Auth;

namespace Forge.Tests.Contracts.Auth;

public class RegisterDtoTests
{
    [Test]
    public void RegisterRequest_ShouldStoreRegistrationFields()
    {
        var request = new RegisterDto
        {
            Login = "jdoe",
            Email = "jdoe@example.com",
            Password = "secret"
        };

        Assert.Multiple(() =>
        {
            Assert.That(request.Login, Is.EqualTo("jdoe"));
            Assert.That(request.Email, Is.EqualTo("jdoe@example.com"));
            Assert.That(request.Password, Is.EqualTo("secret"));
        });
    }
}
