using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class Sprint
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public required DateOnly StartDate { get; set; }
    
    [Required]
    public required DateOnly EndDate { get; set; }

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
