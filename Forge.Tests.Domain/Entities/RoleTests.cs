using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class RoleTests
{
    [Test]
    public void Role_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var deletedAt = DateTime.UtcNow;

        var role = new Role
        {
            Id = 3,
            Name = "Admin",
            DeletedAt = deletedAt
        };

        Assert.Multiple(() =>
        {
            Assert.That(role.Id, Is.EqualTo(3));
            Assert.That(role.Name, Is.EqualTo("Admin"));
            Assert.That(role.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(role.Permissions, Is.Not.Null);
            Assert.That(role.Users, Is.Not.Null);
            Assert.That(role.Permissions, Is.Empty);
            Assert.That(role.Users, Is.Empty);
        });
    }
}
