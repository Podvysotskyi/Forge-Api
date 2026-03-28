using Forge.Domain.Entities;

namespace Forge.Services.Abstractions;

public interface IRoleService
{
    Task<List<Role>> GetAll(CancellationToken cancellationToken = default);
}