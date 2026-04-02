using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class Task
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? ParentTaskId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public int TaskStatusId { get; set; }

    public int TaskPriorityId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [Required]
    public required DateTime CreatedAt { get; set; }

    [Required]
    public required DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    #region Relationships

    public virtual Task? ParentTask { get; set; }

    public virtual Project? Project { get; set; }

    public virtual TaskStatus? TaskStatus { get; set; }

    public virtual TaskPriority? TaskPriority { get; set; }

    public virtual ICollection<Epic> Epics { get; set; } = [];

    public virtual ICollection<Sprint> Sprints { get; set; } = [];

    public virtual ICollection<Task> Tasks { get; set; } = [];

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = [];
    
    public virtual ICollection<TaskLabel> TaskLabels { get; set; } = [];

    #endregion
}
