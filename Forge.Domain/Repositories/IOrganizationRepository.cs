using Forge.Domain.Entities;

namespace Forge.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<List<Organization>> GetAll(CancellationToken cancellationToken = default);
    Task<Organization> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<bool> NameExists(string name, CancellationToken cancellationToken = default);
    Task<Organization> Create(string name, CancellationToken cancellationToken = default);
    Task<Organization> Create(User user, CancellationToken cancellationToken = default);
    Task<Organization> Delete(Organization organization, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationUser>> LoadUsers(Organization organization, CancellationToken cancellationToken = default);
}
