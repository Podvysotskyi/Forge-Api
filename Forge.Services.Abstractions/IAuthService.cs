using Forge.Contracts.Data.Auth;
using Forge.Domain.Entities;

namespace Forge.Services.Abstractions;

public interface IAuthService {
    Task<User> Login(LoginDto dto, CancellationToken cancellationToken = default);

    Task<User> Register(RegisterDto dto, CancellationToken cancellationToken = default);
}
