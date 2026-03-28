using Forge.Domain.Entities;
using Forge.Domain.Repositories;
using Forge.Services.Abstractions;

namespace Forge.Services;

public class RoleService(IRepositoryManager repositoryManager) : IRoleService
{
    public async Task<List<Role>> GetAll(CancellationToken cancellationToken = default)
    {
        return await repositoryManager.RoleRepository.UserRoles(cancellationToken);
    }
}
