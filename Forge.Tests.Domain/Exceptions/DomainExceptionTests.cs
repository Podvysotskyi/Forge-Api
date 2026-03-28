using Forge.Domain.Exceptions;

namespace Forge.Tests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Test]
    public void DomainException_ShouldPreserveProvidedMessage()
    {
        var exception = new TestDomainException("domain");

        Assert.That(exception.Message, Is.EqualTo("domain"));
    }

    private sealed class TestDomainException(string message) : DomainException(message);
}
