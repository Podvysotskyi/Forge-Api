using System.ComponentModel.DataAnnotations;

namespace Forge.Domain.Entities;

public class Organization {
    public Guid Id { get; set; }

    [Required] [MaxLength(45)] public required string Name { get; set; }

    public Guid? UserId { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual User? User { get; set; }
    
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];

    #endregion
}
