using Forge.Contracts.Data.Auth;
using Forge.Contracts.Types.Role;
using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Auth;
using Forge.Domain.Exceptions.User;
using Forge.Domain.Repositories;
using Forge.Services.Abstractions;

namespace Forge.Services;

public class AuthService(
    IRepositoryManager repositoryManager,
    IPasswordService passwordService
) : IAuthService {
    private static readonly UserRole DefaultRole = UserRole.User;
    private static readonly UserStatus DefaultUserStatus = UserStatus.Active;

    public async Task<User> Login(LoginDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await repositoryManager.UserRepository.GetByLoginOrEmail(dto.LoginOrEmail, cancellationToken);

            if (!passwordService.VerifyPassword(user.Password, dto.Password))
            {
                throw new InvalidCredentialsException();
            }

            return user;
        }
        catch (UserDoesNotExistsException)
        {
            throw new InvalidCredentialsException();
        }
    }

    public async Task<User> Register(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var loginExists = await repositoryManager.UserRepository.LoginExists(dto.Login, cancellationToken);
        var emailExists = await repositoryManager.UserRepository.EmailExists(dto.Email, cancellationToken);

        if (loginExists || emailExists)
        {
            throw new UserAlreadyExistsException();
        }

        var userStatus = await repositoryManager.StatusRepository.Find(DefaultUserStatus, cancellationToken);

        var user = await repositoryManager.UserRepository.Create(
            login: dto.Login,
            email: dto.Email,
            password: passwordService.HashPassword(dto.Password),
            userStatus,
            cancellationToken
        );
        var defaultRole = await repositoryManager.RoleRepository.Find(DefaultRole, cancellationToken);

        user.Roles.Add(defaultRole);

        return user;
    }
}
