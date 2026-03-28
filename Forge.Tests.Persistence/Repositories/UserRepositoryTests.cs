using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.User;
using Forge.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Tests.Persistence.Repositories;

public class UserRepositoryTests
{
    [Test]
    public async Task LoginExists_ShouldReturnTrue_WhenUserWithLoginExistsAndIsNotDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "JdOe"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.LoginExists("jdoe");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task LoginExists_ShouldReturnTrue_WhenUserWithLoginExistsAndIsDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "jdoe", deletedAt: DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.LoginExists("jdoe");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task LoginExists_ShouldReturnFalse_WhenUserIsMissing()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var repository = new UserRepository(dbContext);

        var result = await repository.LoginExists("missing");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task EmailExists_ShouldReturnTrue_WhenUserWithEmailExistsAndIsNotDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(email: "JdOe@Example.com"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.EmailExists("jdoe@example.com");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task EmailExists_ShouldReturnTrue_WhenUserWithEmailExistsAndIsDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(email: "jdoe@example.com", deletedAt: DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.EmailExists("jdoe@example.com");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task EmailExists_ShouldReturnFalse_WhenUserIsMissing()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var repository = new UserRepository(dbContext);

        var result = await repository.EmailExists("missing@example.com");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetByLogin_ShouldReturnUser_WhenUserExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "JdOe"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.GetByLogin("jdoe");

        Assert.That(result.Email, Is.EqualTo("jdoe@example.com"));
    }

    [Test]
    public void GetByLogin_ShouldThrowUserDoesNotExistsException_WhenUserIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "jdoe", deletedAt: DateTime.UtcNow));
        dbContext.SaveChanges();

        var repository = new UserRepository(dbContext);

        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByLogin("jdoe"));
        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByLogin("missing"));
    }

    [Test]
    public async Task GetByEmail_ShouldReturnUser_WhenUserExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(email: "JdOe@Example.com"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.GetByEmail("jdoe@example.com");

        Assert.That(result.Login, Is.EqualTo("jdoe"));
    }

    [Test]
    public void GetByEmail_ShouldThrowUserDoesNotExistsException_WhenUserIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(email: "jdoe@example.com", deletedAt: DateTime.UtcNow));
        dbContext.SaveChanges();

        var repository = new UserRepository(dbContext);

        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByEmail("jdoe@example.com"));
        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByEmail("missing@example.com"));
    }

    [Test]
    public async Task GetByLoginOrEmail_ShouldReturnUser_WhenLoginMatches()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "JdOe"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.GetByLoginOrEmail("jdoe");

        Assert.That(result.Login, Is.EqualTo("JdOe"));
    }

    [Test]
    public async Task GetByLoginOrEmail_ShouldReturnUser_WhenEmailMatches()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(email: "JdOe@Example.com"));
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        var result = await repository.GetByLoginOrEmail("jdoe@example.com");

        Assert.That(result.Email, Is.EqualTo("JdOe@Example.com"));
    }

    [Test]
    public void GetByLoginOrEmail_ShouldThrowUserDoesNotExistsException_WhenUserIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Users.Add(CreateUser(login: "jdoe", email: "jdoe@example.com", deletedAt: DateTime.UtcNow));
        dbContext.SaveChanges();

        var repository = new UserRepository(dbContext);

        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByLoginOrEmail("jdoe"));
        Assert.ThrowsAsync<UserDoesNotExistsException>(() => repository.GetByLoginOrEmail("jdoe@example.com"));
    }

    [Test]
    public async Task Create_ShouldAddUserToContextWithoutSavingChanges()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var repository = new UserRepository(dbContext);
        var status = new Status { Id = 42, Name = nameof(UserStatus.Active) };

        var result = await repository.Create("jdoe", "jdoe@example.com", "hashed-password", status);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Login, Is.EqualTo("jdoe"));
            Assert.That(result.Email, Is.EqualTo("jdoe@example.com"));
            Assert.That(result.Password, Is.EqualTo("hashed-password"));
            Assert.That(result.StatusId, Is.EqualTo(42));
            Assert.That(result.Status, Is.SameAs(status));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.UpdatedAt, Is.EqualTo(result.CreatedAt));
            Assert.That(dbContext.Entry(result).State, Is.EqualTo(EntityState.Added));
            Assert.That(dbContext.Users.Local.Single().Login, Is.EqualTo("jdoe"));
        });
    }

    [Test]
    public async Task LoadPermissions_ShouldLoadPermissionsRelationOnUserModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var user = CreateUser();
        var activePermission = new Permission { Id = 1, Name = "Users.Read" };
        var deletedPermission = new Permission { Id = 2, Name = "Users.Delete", DeletedAt = DateTime.UtcNow };

        user.Permissions.Add(activePermission);
        user.Permissions.Add(deletedPermission);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new UserRepository(dbContext);
        var persistedUser = await repository.GetByLogin("jdoe");

        var result = (await repository.LoadPermissions(persistedUser)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.Select(permission => permission.Name), Is.EqualTo(["Users.Read"]));
            Assert.That(persistedUser.Permissions.Select(permission => permission.Name), Is.EqualTo(["Users.Read"]));
            Assert.That(result, Is.EqualTo(persistedUser.Permissions));
        });
    }

    [Test]
    public async Task LoadRoles_ShouldLoadRolesRelationOnUserModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var user = CreateUser();
        var activeRole = new Role { Id = 1, Name = "Admin" };
        var deletedRole = new Role { Id = 2, Name = "Deleted", DeletedAt = DateTime.UtcNow };

        user.Roles.Add(activeRole);
        user.Roles.Add(deletedRole);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new UserRepository(dbContext);
        var persistedUser = await repository.GetByLogin("jdoe");

        var result = (await repository.LoadRoles(persistedUser)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.Select(role => role.Name), Is.EqualTo(["Admin"]));
            Assert.That(persistedUser.Roles.Select(role => role.Name), Is.EqualTo(["Admin"]));
            Assert.That(result, Is.EqualTo(persistedUser.Roles));
        });
    }

    [Test]
    public async Task LoadStatus_ShouldLoadStatusReferenceOnUserModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active" });
        dbContext.Users.Add(CreateUser(statusId: 1));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new UserRepository(dbContext);
        var persistedUser = await repository.GetByLogin("jdoe");

        var result = await repository.LoadStatus(persistedUser);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo("Active"));
            Assert.That(persistedUser.Status, Is.Not.Null);
            Assert.That(persistedUser.Status!.Name, Is.EqualTo("Active"));
        });
    }

    private static User CreateUser(
        string login = "jdoe",
        string email = "jdoe@example.com",
        int statusId = 1,
        DateTime? deletedAt = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email,
            Password = "hashed-password",
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = deletedAt
        };
    }
}
