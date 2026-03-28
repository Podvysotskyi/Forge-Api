using System.ComponentModel.DataAnnotations;
using Forge.Contracts.Types.Status;
using Forge.Domain.Exceptions.User;

namespace Forge.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required][MaxLength(45)] public required string Login { get; set; }

    [Required][MaxLength(100)] public required string Email { get; set; }

    public DateTime? EmailConfirmedAt { get; set; }

    [Required][MaxLength(100)] public required string Password { get; set; }

    public required int StatusId { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual ICollection<Role> Roles { get; set; } = [];

    public virtual ICollection<Permission> Permissions { get; set; } = [];

    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];
    
    public virtual Organization? Organization { get; set; }

    public virtual Status? Status { get; set; }

    #endregion
    
    public bool IsActive => Status != null ? Status?.Name == nameof(UserStatus.Active) : throw new UserStatusIsNotLoadedException();
}
