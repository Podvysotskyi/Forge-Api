using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class OrganizationTests
{
    [Test]
    public void Organization_ShouldInitializeScalarPropertiesAndOptionalUserRelationship()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(5);
        var deletedAt = createdAt.AddDays(1);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "jdoe",
            Email = "jdoe@example.com",
            Password = "hashed-password",
            StatusId = 1,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            UserId = user.Id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            User = user
        };

        Assert.Multiple(() =>
        {
            Assert.That(organization.Name, Is.EqualTo("Acme"));
            Assert.That(organization.UserId, Is.EqualTo(user.Id));
            Assert.That(organization.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(organization.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(organization.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(organization.User, Is.SameAs(user));
        });
    }
}
