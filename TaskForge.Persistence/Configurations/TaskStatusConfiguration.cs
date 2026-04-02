using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatusEntity = TaskForge.Domain.Entities.TaskStatus;

namespace TaskForge.Persistence.Configurations;

internal sealed class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<TaskStatusEntity> builder)
    {
        builder.HasKey(taskStatus => taskStatus.Id);

        builder.Property(taskStatus => taskStatus.Name)
            .IsRequired()
            .HasMaxLength(45);

        builder.HasIndex(taskStatus => taskStatus.Name)
            .IsUnique();

        builder.HasMany(taskStatus => taskStatus.Tasks)
            .WithOne(task => task.TaskStatus)
            .HasForeignKey(task => task.TaskStatusId)
            .IsRequired();
    }
}
