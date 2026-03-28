using Forge.Domain.Entities;
using Forge.Domain.Exceptions.User;
using Forge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forge.Persistence.Repositories;

public class UserRepository(RepositoryDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public async Task<bool> LoginExists(string login, CancellationToken cancellationToken = default)
    {
        var normalizedLogin = login.ToLower();

        return await Entities
            .AnyAsync(user => user.Login.ToLower() == normalizedLogin, cancellationToken);
    }

    public async Task<bool> EmailExists(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLower();

        return await Entities
            .AnyAsync(user => user.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<User> GetByLogin(string login, CancellationToken cancellationToken = default)
    {
        var normalizedLogin = login.ToLower();

        return await Entities
            .FirstOrDefaultAsync(user => user.Login.ToLower() == normalizedLogin && user.DeletedAt == null, cancellationToken)
            ?? throw new UserDoesNotExistsException();
    }

    public async Task<User> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLower();

        return await Entities
            .FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail && user.DeletedAt == null, cancellationToken)
            ?? throw new UserDoesNotExistsException();
    }

    public async Task<User> GetByLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken = default)
    {
        var normalizedLoginOrEmail = loginOrEmail.ToLower();

        return await Entities
            .FirstOrDefaultAsync(user =>
                (user.Login.ToLower() == normalizedLoginOrEmail || user.Email.ToLower() == normalizedLoginOrEmail) &&
                user.DeletedAt == null,
                cancellationToken)
            ?? throw new UserDoesNotExistsException();
    }

    public async Task<User> Create(string login, string email, string password, Status userStatus, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email,
            Password = password,
            StatusId = userStatus.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        await Entities.AddAsync(user, cancellationToken);

        user.Status = userStatus;
        
        return user;
    }

    public async Task<IEnumerable<Permission>> LoadPermissions(User user, CancellationToken cancellationToken = default)
    {
        if (!Entry(user).Collection(currentUser => currentUser.Permissions).IsLoaded)
        {
            await Entry(user)
                .Collection(currentUser => currentUser.Permissions)
                .Query()
                .Where(permission => permission.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return user.Permissions.ToList();
    }

    public async Task<IEnumerable<Role>> LoadRoles(User user, CancellationToken cancellationToken = default)
    {
        if (!Entry(user).Collection(currentUser => currentUser.Roles).IsLoaded)
        {
            await Entry(user)
                .Collection(currentUser => currentUser.Roles)
                .Query()
                .Where(role => role.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return user.Roles.ToList();
    }

    public async Task<Status> LoadStatus(User user, CancellationToken cancellationToken = default)
    {
        if (!Entry(user).Reference(currentUser => currentUser.Status).IsLoaded)
        {
            await Entry(user)
                .Reference(currentUser => currentUser.Status)
                .Query()
                .Where(userStatus => userStatus.DeletedAt == null)
                .LoadAsync(cancellationToken);
        }

        return user.Status!;
    }
}
