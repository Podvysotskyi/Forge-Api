using Forge.Contracts.Data.Organization;
using Forge.Domain.Entities;

namespace Forge.Services.Abstractions;

public interface IOrganizationService
{
    Task<List<Organization>> GetAll(CancellationToken cancellationToken = default);
    Task<Organization> Create(CreateOrganizationDto dto, User user, CancellationToken cancellationToken = default);
    Task<Organization> Create(User user, CancellationToken cancellationToken = default);
}
