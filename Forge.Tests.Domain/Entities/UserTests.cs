using Forge.Domain.Entities;

namespace Forge.Tests.Domain.Entities;

public class UserTests
{
    [Test]
    public void User_ShouldInitializeScalarPropertiesRelationshipsAndOrganizationNavigation()
    {
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddMinutes(5);
        var deletedAt = createdAt.AddDays(1);
        var emailConfirmedAt = createdAt.AddMinutes(10);
        var status = new Status
        {
            Id = 1,
            Name = "Active"
        };
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            UserId = Guid.Empty,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "jdoe",
            Email = "jdoe@example.com",
            EmailConfirmedAt = emailConfirmedAt,
            Password = "hashed-password",
            StatusId = status.Id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            Organization = organization,
            Status = status
        };

        organization.UserId = user.Id;
        organization.User = user;

        Assert.Multiple(() =>
        {
            Assert.That(user.Login, Is.EqualTo("jdoe"));
            Assert.That(user.Email, Is.EqualTo("jdoe@example.com"));
            Assert.That(user.EmailConfirmedAt, Is.EqualTo(emailConfirmedAt));
            Assert.That(user.Password, Is.EqualTo("hashed-password"));
            Assert.That(user.StatusId, Is.EqualTo(1));
            Assert.That(user.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(user.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(user.DeletedAt, Is.EqualTo(deletedAt));
            Assert.That(user.Status, Is.SameAs(status));
            Assert.That(user.Organization, Is.SameAs(organization));
            Assert.That(user.Roles, Is.Not.Null);
            Assert.That(user.Permissions, Is.Not.Null);
            Assert.That(user.Roles, Is.Empty);
            Assert.That(user.Permissions, Is.Empty);
        });
    }
}
