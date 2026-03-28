using System.ComponentModel.DataAnnotations;

namespace Forge.Domain.Entities;

public class Permission
{
    public int Id { get; set; }

    [Required][MaxLength(45)] public required string Name { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual ICollection<User> Users { get; set; } = [];

    public virtual ICollection<Role> Roles { get; set; } = [];
    
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];

    #endregion
}
