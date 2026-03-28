using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Status;
using Forge.Persistence.Repositories;

namespace Forge.Tests.Persistence.Repositories;

public class StatusRepositoryTests
{
    [Test]
    public async Task GetUserStatuses_ShouldReturnOnlyNonDeletedStatusesDefinedByUserStatusType()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.AddRange(
            new Status { Id = 1, Name = "Active" },
            new Status { Id = 2, Name = "Deleted", DeletedAt = DateTime.UtcNow },
            new Status { Id = 3, Name = "Blocked" },
            new Status { Id = 4, Name = "OrganizationActive" });
        await dbContext.SaveChangesAsync();

        var repository = new StatusRepository(dbContext);

        var result = (await repository.GetUserStatuses()).ToArray();

        Assert.That(result.Select(status => status.Name), Is.EqualTo(["Active", "Blocked"]));
    }

    [Test]
    public async Task GetOrganizationUserStatuses_ShouldReturnOnlyNonDeletedStatusesDefinedByOrganizationUserStatusType()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.AddRange(
            new Status { Id = 1, Name = "Active" },
            new Status { Id = 2, Name = "OrganizationActive" },
            new Status { Id = 3, Name = "OrganizationBlocked" },
            new Status { Id = 4, Name = "OrganizationPending", DeletedAt = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var repository = new StatusRepository(dbContext);

        var result = (await repository.GetOrganizationUserStatuses()).ToArray();

        Assert.That(result.Select(status => status.Name), Is.EqualTo(["OrganizationActive", "OrganizationBlocked"]));
    }

    [Test]
    public async Task GetById_ShouldReturnStatus_WhenStatusExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active" });
        await dbContext.SaveChangesAsync();

        var repository = new StatusRepository(dbContext);

        var result = await repository.GetById(1);

        Assert.That(result.Name, Is.EqualTo("Active"));
    }

    [Test]
    public void GetById_ShouldThrowUserStatusDoesNotExistsException_WhenStatusIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new StatusRepository(dbContext);

        Assert.ThrowsAsync<StatusDoesNotExistsException>(() => repository.GetById(1));
        Assert.ThrowsAsync<StatusDoesNotExistsException>(() => repository.GetById(2));
    }

    [Test]
    public async Task Find_UserStatus_ShouldReturnStatus_WhenMatchingStatusExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active" });
        await dbContext.SaveChangesAsync();

        var repository = new StatusRepository(dbContext);

        var result = await repository.Find(UserStatus.Active);

        Assert.That(result.Name, Is.EqualTo("Active"));
    }

    [Test]
    public void Find_UserStatus_ShouldThrowUserStatusDoesNotExistsException_WhenStatusIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "Active", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new StatusRepository(dbContext);

        Assert.ThrowsAsync<StatusDoesNotExistsException>(() => repository.Find(UserStatus.Active));
    }

    [Test]
    public async Task Find_OrganizationUserStatus_ShouldReturnStatus_WhenMatchingStatusExists()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "OrganizationActive" });
        await dbContext.SaveChangesAsync();

        var repository = new StatusRepository(dbContext);

        var result = await repository.Find(OrganizationUserStatus.OrganizationActive);

        Assert.That(result.Name, Is.EqualTo("OrganizationActive"));
    }

    [Test]
    public void Find_OrganizationUserStatus_ShouldThrowUserStatusDoesNotExistsException_WhenStatusIsMissingOrDeleted()
    {
        using var dbContext = RepositoryDbContextTestFactory.Create();
        dbContext.Statuses.Add(new Status { Id = 1, Name = "OrganizationActive", DeletedAt = DateTime.UtcNow });
        dbContext.SaveChanges();

        var repository = new StatusRepository(dbContext);

        Assert.ThrowsAsync<StatusDoesNotExistsException>(() => repository.Find(OrganizationUserStatus.OrganizationActive));
    }
}
