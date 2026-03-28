using System.ComponentModel.DataAnnotations;

namespace Forge.Domain.Entities;

public class Role
{
    public int Id { get; set; }

    [Required][MaxLength(45)] public required string Name { get; set; }
        
    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual ICollection<Permission> Permissions { get; set; } = [];

    public virtual ICollection<User> Users { get; set; } = [];
        
    #endregion
}
