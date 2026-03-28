using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class OrganizationUserTests
{
    [Test]
    public void OrganizationUser_ShouldInitializeForeignKeysTimestampsDeletedAtAndNavigationProperties()
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "jdoe",
            Email = "jdoe@example.com",
            Password = "hashed-password",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(5);
        var deletedAt = DateTime.UtcNow;
        var role = new Role
        {
            Id = 4,
            Name = "User"
        };
        var status = new Status
        {
            Id = 1,
            Name = "Active"
        };

        var organizationUser = new OrganizationUser
        {
            OrganizationId = organization.Id,
            UserId = user.Id,
            StatusId = status.Id,
            RoleId = role.Id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            Organization = organization,
            User = user,
            Status = status,
            Role = role
        };

        Assert.Multiple(() =>
        {
            Assert.That(organizationUser.OrganizationId, Is.EqualTo(organization.Id));
            Assert.That(organizationUser.UserId, Is.EqualTo(user.Id));
            Assert.That(organizationUser.StatusId, Is.EqualTo(status.Id));
            Assert.That(organizationUser.RoleId, Is.EqualTo(role.Id));
            Assert.That(organizationUser.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(organizationUser.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(organizationUser.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(organizationUser.Organization, Is.SameAs(organization));
            Assert.That(organizationUser.User, Is.SameAs(user));
            Assert.That(organizationUser.Status, Is.SameAs(status));
            Assert.That(organizationUser.Role, Is.SameAs(role));
            Assert.That(organizationUser.Permissions, Is.Not.Null);
            Assert.That(organizationUser.Permissions, Is.Empty);
        });
    }
}
