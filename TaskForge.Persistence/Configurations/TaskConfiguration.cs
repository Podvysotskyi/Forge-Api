using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = TaskForge.Domain.Entities.Task;

namespace TaskForge.Persistence.Configurations;

internal sealed class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(task => task.Description)
            .HasMaxLength(1000);

        builder.HasIndex(task => new { task.ProjectId, task.Name });

        builder.HasOne(task => task.Project)
            .WithMany(project => project.Tasks)
            .HasForeignKey(task => task.ProjectId)
            .IsRequired();
        
        builder.HasOne(task => task.TaskStatus)
            .WithMany(taskStatus => taskStatus.Tasks)
            .HasForeignKey(task => task.TaskStatusId)
            .IsRequired();
        
        builder.HasOne(task => task.TaskPriority)
            .WithMany(taskPriority => taskPriority.Tasks)
            .HasForeignKey(task => task.TaskPriorityId)
            .IsRequired();

        builder.HasMany(task => task.Epics)
            .WithMany(epic => epic.Tasks);
        
        builder.HasMany(task => task.Sprints)
            .WithMany(sprint => sprint.Tasks);
        
        builder.HasMany(task => task.Tasks)
            .WithOne(task => task.ParentTask)
            .HasForeignKey(task => task.ParentTaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(task => task.TaskUsers)
            .WithOne(taskUser => taskUser.Task)
            .HasForeignKey(taskUser => taskUser.TaskId)
            .IsRequired();

        builder.HasMany(task => task.TaskLabels)
            .WithMany(taskLabel => taskLabel.Tasks);
    }
}
