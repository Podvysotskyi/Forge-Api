using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class TaskLabel
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(45)]
    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    #region Relationships

    public virtual Project? Project { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = [];

    #endregion
}
