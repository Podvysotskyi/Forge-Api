using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Organization;
using Forge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence.Repositories;

public class OrganizationRepository(RepositoryDbContext dbContext) : Repository<Organization>(dbContext), IOrganizationRepository
{
    public async Task<List<Organization>> GetAll(CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(organization => organization.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Organization> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(organization => organization.Id == id && organization.DeletedAt == null, cancellationToken)
            ?? throw new OrganizationDoesNotExistsException();
    }

    public async Task<bool> NameExists(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.ToLower();

        return await Entities
            .AnyAsync(organization => organization.Name.ToLower() == normalizedName, cancellationToken);
    }

    public async Task<Organization> Create(string name, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = now,
            UpdatedAt = now
        };

        await Entities.AddAsync(organization, cancellationToken);

        return organization;
    }

    public async Task<Organization> Create(User user, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = user.Id.ToString(),
            UserId = user.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        await Entities.AddAsync(organization, cancellationToken);

        organization.User = user;

        return organization;
    }

    public Task<Organization> Delete(Organization organization, CancellationToken cancellationToken = default)
    {
        organization.DeletedAt = DateTime.UtcNow;
        organization.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(organization);
    }

    public async Task<IEnumerable<OrganizationUser>> LoadUsers(Organization organization, CancellationToken cancellationToken = default)
    {
        if (!Entry(organization).Collection(currentOrganization => currentOrganization.OrganizationUsers).IsLoaded)
        {
            await Entry(organization)
                .Collection(currentOrganization => currentOrganization.OrganizationUsers)
                .Query()
                .Where(organizationUser => organizationUser.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        foreach (var organizationUser in organization.OrganizationUsers)
        {
            if (!Entry(organizationUser).Reference(currentOrganizationUser => currentOrganizationUser.User).IsLoaded)
            {
                await Entry(organizationUser)
                    .Reference(currentOrganizationUser => currentOrganizationUser.User)
                    .Query()
                    .Where(currentUser => currentUser.DeletedAt == null)
                    .LoadAsync(cancellationToken);
            }
        }

        return organization.OrganizationUsers;
    }
}
