using Forge.Contracts.Types.Role;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Role;
using Forge.Persistence.Repositories;

namespace Forge.Tests.Persistence.Repositories;

public class RoleRepositoryTests
{
    [Test]
    public async Task UserRoles_ShouldReturnOnlyNonDeletedRolesDefinedByUserRoleType()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Deleted", DeletedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = "User" },
            new Role { Id = 4, Name = "OrganizationOwner" });
        await dbContext.SaveChangesAsync();

        var repository = new RoleRepository(dbContext);

        var result = (await repository.UserRoles()).ToArray();

        Assert.That(result.Select(role => role.Name), Is.EqualTo(["Admin", "User"]));
    }

    [Test]
    public async Task OrganizationUserRoles_ShouldReturnOnlyNonDeletedRolesDefinedByOrganizationUserRoleType()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.AddRange(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "OrganizationOwner" },
            new Role { Id = 3, Name = "OrganizationManager" },
            new Role { Id = 4, Name = "OrganizationUser", DeletedAt = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var repository = new RoleRepository(dbContext);

        var result = (await repository.OrganizationUserRoles()).ToArray();

        Assert.That(result.Select(role => role.Name), Is.EqualTo(["OrganizationOwner", "OrganizationManager"]));
    }

    [Test]
    public async Task GetById_ShouldReturnRole_WhenRoleExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "Admin" });
        await dbContext.SaveChangesAsync();

        var repository = new RoleRepository(dbContext);

        var result = await repository.GetById(1);

        Assert.That(result.Name, Is.EqualTo("Admin"));
    }

    [Test]
    public void GetById_ShouldThrowRoleDoesNotExistsException_WhenRoleIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "Deleted", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new RoleRepository(dbContext);

        Assert.ThrowsAsync<RoleDoesNotExistsException>(() => repository.GetById(1));
        Assert.ThrowsAsync<RoleDoesNotExistsException>(() => repository.GetById(2));
    }

    [Test]
    public async Task Find_UserRoleType_ShouldReturnRole_WhenRoleExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "Admin" });
        await dbContext.SaveChangesAsync();

        var repository = new RoleRepository(dbContext);

        var result = await repository.Find(UserRole.Admin);

        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void Find_UserRoleType_ShouldThrowRoleDoesNotExistsException_WhenRoleIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "Admin", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new RoleRepository(dbContext);

        Assert.ThrowsAsync<RoleDoesNotExistsException>(() => repository.Find(UserRole.Admin));
    }

    [Test]
    public async Task Find_OrganizationUserRole_ShouldReturnRole_WhenRoleExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "OrganizationOwner" });
        await dbContext.SaveChangesAsync();

        var repository = new RoleRepository(dbContext);

        var result = await repository.Find(OrganizationUserRole.OrganizationOwner);

        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void Find_OrganizationUserRole_ShouldThrowRoleDoesNotExistsException_WhenRoleIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Roles.Add(new Role { Id = 1, Name = "OrganizationOwner", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new RoleRepository(dbContext);

        Assert.ThrowsAsync<RoleDoesNotExistsException>(() => repository.Find(OrganizationUserRole.OrganizationOwner));
    }

    [Test]
    public async Task LoadUsers_ShouldLoadUsersRelationOnRoleModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var role = new Role { Id = 1, Name = "Admin" };
        var activeUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Login = "active",
            Email = "active@example.com",
            Password = "password",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var deletedUser = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Login = "deleted",
            Email = "deleted@example.com",
            Password = "password",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = DateTime.UtcNow
        };
        var otherRole = new Role { Id = 2, Name = "User" };
        role.Users.Add(activeUser);
        role.Users.Add(deletedUser);
        otherRole.Users.Add(new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Login = "other",
            Email = "other@example.com",
            Password = "password",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active" });
        dbContext.Roles.AddRange(role, otherRole);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new RoleRepository(dbContext);
        var persistedRole = await repository.GetById(1);

        var result = (await repository.LoadUsers(persistedRole)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.Select(user => user.Login), Is.EqualTo(["active"]));
            Assert.That(persistedRole.Users.Select(user => user.Login), Is.EqualTo(["active"]));
            Assert.That(result, Is.EqualTo(persistedRole.Users));
        });
    }

    [Test]
    public async Task LoadPermissions_ShouldLoadPermissionsRelationOnRoleModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var role = new Role { Id = 1, Name = "Admin" };
        var activePermission = new Permission { Id = 1, Name = "Roles.Read" };
        var deletedPermission = new Permission { Id = 2, Name = "Roles.Delete", DeletedAt = DateTime.UtcNow };
        var otherRole = new Role { Id = 2, Name = "User" };

        role.Permissions.Add(activePermission);
        role.Permissions.Add(deletedPermission);
        otherRole.Permissions.Add(new Permission { Id = 3, Name = "Users.Read" });

        dbContext.Roles.AddRange(role, otherRole);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new RoleRepository(dbContext);
        var persistedRole = await repository.GetById(1);

        var result = (await repository.LoadPermissions(persistedRole)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.Select(permission => permission.Name), Is.EqualTo(["Roles.Read"]));
            Assert.That(persistedRole.Permissions.Select(permission => permission.Name), Is.EqualTo(["Roles.Read"]));
            Assert.That(result, Is.EqualTo(persistedRole.Permissions));
        });
    }
}
