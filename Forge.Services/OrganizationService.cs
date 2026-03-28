using Forge.Contracts.Data.Organization;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Organization;
using Forge.Domain.Repositories;
using Forge.Services.Abstractions;

namespace Forge.Services;

public class OrganizationService(IRepositoryManager repositoryManager) : IOrganizationService
{
    public async Task<List<Organization>> GetAll(CancellationToken cancellationToken = default)
    {
        return await repositoryManager.OrganizationRepository.GetAll(cancellationToken);
    }

    public async Task<Organization> Create(CreateOrganizationDto dto, User user, CancellationToken cancellationToken = default)
    {
        if (await repositoryManager.OrganizationRepository.NameExists(dto.Name, cancellationToken))
        {
            throw new OrganizationAlreadyExistsException();
        }

        var organization = await repositoryManager.OrganizationRepository.Create(dto.Name, cancellationToken);
        
        // TODO: Add user to organization as owner

        return organization;
    }

    public async Task<Organization> Create(User user, CancellationToken cancellationToken = default)
    {
        return await repositoryManager.OrganizationRepository.Create(user, cancellationToken);
    }
}
