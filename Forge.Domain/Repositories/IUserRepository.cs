using Forge.Domain.Entities;

namespace Forge.Domain.Repositories;

public interface IUserRepository {
    Task<bool> LoginExists(string login, CancellationToken cancellationToken = default);
    Task<bool> EmailExists(string email, CancellationToken cancellationToken = default);
    Task<User> GetByLogin(string login, CancellationToken cancellationToken = default);
    Task<User> GetByEmail(string email, CancellationToken cancellationToken = default);
    Task<User> GetByLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken = default);
    Task<User> Create(string login, string email, string password, Status status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> LoadPermissions(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> LoadRoles(User user, CancellationToken cancellationToken = default);
    Task<Status> LoadStatus(User user, CancellationToken cancellationToken = default);
}
