namespace Forge.Domain.Exceptions.User;

public sealed class UserAlreadyExistsException() : DomainException("User already exists");
