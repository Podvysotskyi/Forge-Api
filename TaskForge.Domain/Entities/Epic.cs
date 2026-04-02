using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class Epic
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(45)]
    public required string Name { get; set; }

    public Guid ProjectId { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    [Required]
    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual Project? Project { get; set; }
    
    public virtual ICollection<Task> Tasks { get; set; } = [];

    #endregion
}
