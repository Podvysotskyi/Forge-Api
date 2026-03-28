using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Permission;
using Forge.Persistence.Repositories;

namespace Forge.Tests.Persistence.Repositories;

public class PermissionRepositoryTests
{
    [Test]
    public async Task GetAll_ShouldReturnOnlyNonDeletedPermissions()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.AddRange(
            new Permission { Id = 1, Name = "Users.Read" },
            new Permission { Id = 2, Name = "Users.Delete", DeletedAt = DateTime.UtcNow },
            new Permission { Id = 3, Name = "Users.Write" });
        await dbContext.SaveChangesAsync();

        var repository = new PermissionRepository(dbContext);

        var result = (await repository.GetAll()).ToArray();

        Assert.That(result.Select(permission => permission.Name), Is.EqualTo(["Users.Read", "Users.Write"]));
    }

    [Test]
    public async Task GetById_ShouldReturnPermission_WhenPermissionExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.Add(new Permission { Id = 1, Name = "Users.Read" });
        await dbContext.SaveChangesAsync();

        var repository = new PermissionRepository(dbContext);

        var result = await repository.GetById(1);

        Assert.That(result.Name, Is.EqualTo("Users.Read"));
    }

    [Test]
    public void GetById_ShouldThrowPermissionDoesNotExistsException_WhenPermissionIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.Add(new Permission { Id = 1, Name = "Users.Delete", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new PermissionRepository(dbContext);

        Assert.ThrowsAsync<PermissionDoesNotExistsException>(() => repository.GetById(1));
        Assert.ThrowsAsync<PermissionDoesNotExistsException>(() => repository.GetById(2));
    }

    [Test]
    public async Task GetByName_ShouldReturnPermission_WhenPermissionExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.Add(new Permission { Id = 1, Name = "Users.Read" });
        await dbContext.SaveChangesAsync();

        var repository = new PermissionRepository(dbContext);

        var result = await repository.GetByName("Users.Read");

        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetByName_ShouldThrowPermissionDoesNotExistsException_WhenPermissionIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Permissions.Add(new Permission { Id = 1, Name = "Users.Read", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new PermissionRepository(dbContext);

        Assert.ThrowsAsync<PermissionDoesNotExistsException>(() => repository.GetByName("Users.Read"));
        Assert.ThrowsAsync<PermissionDoesNotExistsException>(() => repository.GetByName("Users.Write"));
    }

    [Test]
    public async Task LoadRoles_ShouldLoadRolesRelationOnPermissionModel()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        var permission = new Permission { Id = 1, Name = "Users.Read" };
        var activeRole = new Role { Id = 1, Name = "Admin" };
        var deletedRole = new Role { Id = 2, Name = "Deleted", DeletedAt = DateTime.UtcNow };
        var otherPermission = new Permission { Id = 2, Name = "Users.Write" };

        permission.Roles.Add(activeRole);
        permission.Roles.Add(deletedRole);
        otherPermission.Roles.Add(new Role { Id = 3, Name = "User" });

        dbContext.Permissions.AddRange(permission, otherPermission);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var repository = new PermissionRepository(dbContext);
        var persistedPermission = await repository.GetById(1);

        var result = (await repository.LoadRoles(persistedPermission)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.Select(role => role.Name), Is.EqualTo(["Admin"]));
            Assert.That(persistedPermission.Roles.Select(role => role.Name), Is.EqualTo(["Admin"]));
            Assert.That(result, Is.EqualTo(persistedPermission.Roles));
        });
    }
}
