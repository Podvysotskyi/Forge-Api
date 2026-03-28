using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Status;
using Forge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence.Repositories;

public class StatusRepository(RepositoryDbContext dbContext) : Repository<Status>(dbContext), IStatusRepository
{
    public async Task<IEnumerable<Status>> GetUserStatuses(CancellationToken cancellationToken = default)
    {
        var names = Enum.GetNames<UserStatus>().AsEnumerable();

        return await Entities
            .Where(userStatus => userStatus.DeletedAt == null && names.Contains(userStatus.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Status>> GetOrganizationUserStatuses(CancellationToken cancellationToken = default)
    {
        var names = Enum.GetNames<OrganizationUserStatus>().AsEnumerable();

        return await Entities
            .Where(userStatus => userStatus.DeletedAt == null && names.Contains(userStatus.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<Status> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(userStatus => userStatus.Id == id && userStatus.DeletedAt == null, cancellationToken)
            ?? throw new StatusDoesNotExistsException();
    }

    public async Task<Status> Find(UserStatus type, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(userStatus => userStatus.Name == type.ToString() && userStatus.DeletedAt == null, cancellationToken)
            ?? throw new StatusDoesNotExistsException();
    }

    public async Task<Status> Find(OrganizationUserStatus type, CancellationToken cancellationToken = default)
    {
        return await Entities
            .FirstOrDefaultAsync(userStatus => userStatus.Name == type.ToString() && userStatus.DeletedAt == null, cancellationToken)
            ?? throw new StatusDoesNotExistsException();
    }
}
