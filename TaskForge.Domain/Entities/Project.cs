using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(45)]
    public required string Name { get; set; }

    public Guid OrganizationId { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    [Required]
    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual ICollection<TaskLabel> TaskLabels { get; set; } = [];
    
    public virtual ICollection<Epic> Epics { get; set; } = [];

    public virtual ICollection<Sprint> Sprints { get; set; } = [];

    public virtual ICollection<Task> Tasks { get; set; } = [];
    
    #endregion
}
