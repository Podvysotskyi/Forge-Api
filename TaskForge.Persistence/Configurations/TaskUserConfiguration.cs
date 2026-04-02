using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class TaskUserConfiguration : IEntityTypeConfiguration<TaskUser>
{
    public void Configure(EntityTypeBuilder<TaskUser> builder)
    {
        builder.HasKey(taskUser => new { taskUser.TaskId, taskUser.UserId });

        builder.HasOne(taskUser => taskUser.Task)
            .WithMany(task => task.TaskUsers)
            .HasForeignKey(taskUser => taskUser.TaskId)
            .IsRequired();
    }
}
