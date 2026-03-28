namespace Forge.Contracts.Data.Auth;

public record LoginDto
{
    public required string LoginOrEmail { get; set; }

    public required string Password { get; set; }
}