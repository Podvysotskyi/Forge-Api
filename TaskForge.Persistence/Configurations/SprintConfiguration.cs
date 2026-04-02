using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.HasKey(sprint => sprint.Id);

        builder.Property(sprint => sprint.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(task => task.Description)
            .HasMaxLength(1000);

        builder.HasIndex(sprint => new { sprint.ProjectId, sprint.Name })
            .IsUnique();

        builder.HasOne(sprint => sprint.Project)
            .WithMany(project => project.Sprints)
            .HasForeignKey(sprint => sprint.ProjectId)
            .IsRequired();

        builder.HasMany(sprint => sprint.Tasks)
            .WithMany(task => task.Sprints);
    }
}
