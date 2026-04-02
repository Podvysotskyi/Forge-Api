using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class TaskPriorityConfiguration : IEntityTypeConfiguration<TaskPriority>
{
    public void Configure(EntityTypeBuilder<TaskPriority> builder)
    {
        builder.HasKey(taskPriority => taskPriority.Id);

        builder.Property(taskPriority => taskPriority.Name)
            .IsRequired()
            .HasMaxLength(45);

        builder.HasIndex(taskPriority => taskPriority.Name)
            .IsUnique();

        builder.HasMany(taskPriority => taskPriority.Tasks)
            .WithOne(task => task.TaskPriority)
            .HasForeignKey(task => task.TaskPriorityId)
            .IsRequired();
    }
}
