using Forge.Domain.Entities;

namespace Forge.Domain.Repositories;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAll(CancellationToken cancellationToken = default);
    Task<Permission> GetById(int id, CancellationToken cancellationToken = default);
    Task<Permission> GetByName(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> LoadRoles(Permission permission, CancellationToken cancellationToken = default); }