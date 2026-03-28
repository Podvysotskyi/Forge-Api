using Forge.Domain.Entities;

namespace Forge.Services.Abstractions;

public interface IPermissionService
{
    Task<List<Permission>> GetAll(CancellationToken cancellationToken = default);
}