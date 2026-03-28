namespace Forge.Domain.Entities;

public class OrganizationUser {
    public required Guid OrganizationId { get; set; }

    public required Guid UserId { get; set; }

    public required int StatusId { get; set; }

    public required int RoleId { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships
    
    public virtual Organization? Organization { get; set; }

    public virtual User? User { get; set; }

    public virtual Status? Status { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = [];
    
    #endregion
}
