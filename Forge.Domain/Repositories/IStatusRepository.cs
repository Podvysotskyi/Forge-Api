using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;

namespace Forge.Domain.Repositories;

public interface IStatusRepository
{
    Task<IEnumerable<Status>> GetUserStatuses(CancellationToken cancellationToken = default);
    Task<IEnumerable<Status>> GetOrganizationUserStatuses(CancellationToken cancellationToken = default);
    Task<Status> GetById(int id, CancellationToken cancellationToken = default);
    Task<Status> Find(UserStatus type, CancellationToken cancellationToken = default);
    Task<Status> Find(OrganizationUserStatus type, CancellationToken cancellationToken = default); 
}
