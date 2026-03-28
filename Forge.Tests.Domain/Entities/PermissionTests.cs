using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class PermissionTests
{
    [Test]
    public void Permission_ShouldInitializeScalarPropertiesAndRelationshipCollections()
    {
        var deletedAt = DateTime.UtcNow;

        var permission = new Permission
        {
            Id = 7,
            Name = "ManageUsers",
            DeletedAt = deletedAt
        };

        Assert.Multiple(() =>
        {
            Assert.That(permission.Id, Is.EqualTo(7));
            Assert.That(permission.Name, Is.EqualTo("ManageUsers"));
            Assert.That(permission.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(permission.Users, Is.Not.Null);
            Assert.That(permission.Roles, Is.Not.Null);
            Assert.That(permission.Users, Is.Empty);
            Assert.That(permission.Roles, Is.Empty);
        });
    }
}
