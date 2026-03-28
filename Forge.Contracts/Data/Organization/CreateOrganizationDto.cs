namespace Forge.Contracts.Data.Organization;

public record CreateOrganizationDto
{
    public required string Name { get; init; }
}
