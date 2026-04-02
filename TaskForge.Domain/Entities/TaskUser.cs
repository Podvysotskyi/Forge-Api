namespace TaskForge.Domain.Entities;

public class TaskUser
{
    public Guid TaskId { get; set; }
    
    public Guid UserId { get; set; }

    public required DateTime CreatedAt { get; set; }

    #region Relationships

    public virtual Task? Task { get; set; }

    #endregion
}
