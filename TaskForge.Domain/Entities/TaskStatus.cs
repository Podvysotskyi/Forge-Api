using System.ComponentModel.DataAnnotations;

namespace TaskForge.Domain.Entities;

public class TaskStatus
{
    public int Id { get; set; }

    [Required]
    [MaxLength(45)]
    public required string Name { get; set; }

    #region Relationships

    public virtual ICollection<Task> Tasks { get; set; } = [];

    #endregion
}
