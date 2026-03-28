using Forge.Domain.Entities;
using Forge.Domain.Repositories;
using Forge.Services.Abstractions;

namespace Forge.Services;

public class PermissionService(IRepositoryManager repositoryManager) : IPermissionService
{
    public async Task<List<Permission>> GetAll(CancellationToken cancellationToken = default)
    {
        return await repositoryManager.PermissionRepository.GetAll(cancellationToken);
    }
}
