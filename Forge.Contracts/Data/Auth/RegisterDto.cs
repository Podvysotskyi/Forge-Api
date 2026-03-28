namespace Forge.Contracts.Data.Auth;

public record RegisterDto
{
    public required string Login { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}
