using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class TaskLabelConfiguration : IEntityTypeConfiguration<TaskLabel>
{
    public void Configure(EntityTypeBuilder<TaskLabel> builder)
    {
        builder.HasKey(taskLabel => taskLabel.Id);

        builder.Property(taskLabel => taskLabel.Name)
            .IsRequired()
            .HasMaxLength(45);

        builder.HasIndex(taskLabel => new { taskLabel.ProjectId, taskLabel.Name })
            .IsUnique();

        builder.HasOne(taskLabel => taskLabel.Project)
            .WithMany(project => project.TaskLabels)
            .HasForeignKey(taskLabel => taskLabel.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(taskLabel => taskLabel.Tasks)
            .WithMany(task => task.TaskLabels);
    }
}
