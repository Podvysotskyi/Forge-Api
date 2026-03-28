using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class StatusTests
{
    [Test]
    public void Status_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var deletedAt = DateTime.UtcNow;

        var status = new Status
        {
            Id = 2,
            Name = "Pending",
            DeletedAt = deletedAt
        };

        Assert.Multiple(() =>
        {
            Assert.That(status.Id, Is.EqualTo(2));
            Assert.That(status.Name, Is.EqualTo("Pending"));
            Assert.That(status.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(status.Users, Is.Not.Null);
            Assert.That(status.Users, Is.Empty);
            Assert.That(status.OrganizationUsers, Is.Not.Null);
            Assert.That(status.OrganizationUsers, Is.Empty);
        });
    }
}
