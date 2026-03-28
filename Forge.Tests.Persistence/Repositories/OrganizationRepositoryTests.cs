using Forge.Contracts.Types.Role;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Organization;
using Forge.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Tests.Persistence.Repositories;

public class OrganizationRepositoryTests
{
    [Test]
    public async Task GetAll_ShouldReturnOnlyNonDeletedOrganizations()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Organizations.AddRange(
            CreateOrganization(name: "Acme"),
            CreateOrganization(name: "Deleted", deletedAt: DateTime.UtcNow),
            CreateOrganization(name: "Globex"));
        await dbContext.SaveChangesAsync();

        var repository = new OrganizationRepository(dbContext);

        var result = (await repository.GetAll()).ToArray();

        Assert.That(result.Select(organization => organization.Name), Is.EqualTo(["Acme", "Globex"]));
    }

    [Test]
    public async Task GetById_ShouldReturnOrganization_WhenOrganizationExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var organization = CreateOrganization(name: "Acme");
        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesAsync();

        var repository = new OrganizationRepository(dbContext);

        var result = await repository.GetById(organization.Id);

        Assert.That(result.Name, Is.EqualTo("Acme"));
    }

    [Test]
    public void GetById_ShouldThrowOrganizationDoesNotExistsException_WhenOrganizationIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var organization = CreateOrganization(name: "Acme", deletedAt: DateTime.UtcNow);
        dbContext.Organizations.Add(organization);
        dbContext.SaveChanges();

        var repository = new OrganizationRepository(dbContext);

        Assert.ThrowsAsync<OrganizationDoesNotExistsException>(() => repository.GetById(organization.Id));
        Assert.ThrowsAsync<OrganizationDoesNotExistsException>(() => repository.GetById(Guid.NewGuid()));
    }

    [Test]
    public async Task NameExists_ShouldCompareCaseInsensitivelyAcrossDeletedAndNonDeletedOrganizations()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Organizations.AddRange(
            CreateOrganization(name: "AcMe"),
            CreateOrganization(name: "Deleted", deletedAt: DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var repository = new OrganizationRepository(dbContext);

        Assert.Multiple(async () =>
        {
            Assert.That(await repository.NameExists("acme"), Is.True);
            Assert.That(await repository.NameExists("deleted"), Is.True);
            Assert.That(await repository.NameExists("missing"), Is.False);
        });
    }

    [Test]
    public async Task Create_WithDto_ShouldAddOrganizationToContextWithoutSavingChanges()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var repository = new OrganizationRepository(dbContext);

        var result = await repository.Create("Acme");

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo("Acme"));
            Assert.That(result.UserId, Is.Null);
            Assert.That(result.User, Is.Null);
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.UpdatedAt, Is.EqualTo(result.CreatedAt));
            Assert.That(dbContext.Entry(result).State, Is.EqualTo(EntityState.Added));
        });
    }

    [Test]
    public async Task Create_WithUser_ShouldUseUserIdAsOrganizationName()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var user = CreateUser();
        var repository = new OrganizationRepository(dbContext);

        var result = await repository.Create(user);

        Assert.That(result.Name, Is.EqualTo(user.Id.ToString()));
    }

    [Test]
    public async Task Delete_ShouldSetDeletedAtAndUpdatedAt()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var organization = CreateOrganization();
        var originalUpdatedAt = organization.UpdatedAt;
        var repository = new OrganizationRepository(dbContext);

        var result = await repository.Delete(organization);

        Assert.Multiple(() =>
        {
            Assert.That(result.DeletedAt, Is.Not.Null);
            Assert.That(result.UpdatedAt, Is.GreaterThanOrEqualTo(originalUpdatedAt));
        });
    }

    [Test]
    public async Task LoadUsers_ShouldReturnOrganizationUsersCollectionOnOrganizationModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var organization = CreateOrganization();
        var status = new Status
        {
            Id = 1,
            Name = "Active"
        };
        var activeUser = CreateUser(login: "active", email: "active@example.com", status: status);
        var deletedUser = CreateUser(login: "deleted", email: "deleted@example.com", deletedAt: DateTime.UtcNow, status: status);
        organization.OrganizationUsers.Add(new OrganizationUser
        {
            OrganizationId = organization.Id,
            Organization = organization,
            UserId = activeUser.Id,
            User = activeUser,
            StatusId = activeUser.StatusId,
            RoleId = (int)UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        organization.OrganizationUsers.Add(new OrganizationUser
        {
            OrganizationId = organization.Id,
            Organization = organization,
            UserId = deletedUser.Id,
            User = deletedUser,
            StatusId = deletedUser.StatusId,
            RoleId = (int)UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        dbContext.Organizations.Add(organization);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new OrganizationRepository(dbContext);
        var persistedOrganization = await repository.GetById(organization.Id);

        var result = (await repository.LoadUsers(persistedOrganization)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(persistedOrganization.OrganizationUsers));
            Assert.That(result.Select(organizationUser => organizationUser.User?.Login), Is.EqualTo(["active", null]));
        });
    }

    private static Organization CreateOrganization(string name = "Acme", DateTime? deletedAt = null)
    {
        var now = DateTime.UtcNow;

        return new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = now,
            UpdatedAt = now,
            DeletedAt = deletedAt
        };
    }

    private static User CreateUser(
        string login = "jdoe",
        string email = "jdoe@example.com",
        DateTime? deletedAt = null,
        Status? status = null)
    {
        status ??= new Status
        {
            Id = 1,
            Name = "Active"
        };

        return new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email,
            Password = "hashed-password",
            StatusId = status.Id,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = deletedAt
        };
    }
}
