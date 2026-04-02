using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .IsRequired()
            .HasMaxLength(45);

        builder.HasIndex(project => new { project.OrganizationId, project.Name })
            .IsUnique();

        builder.HasMany(project => project.TaskLabels)
            .WithOne(taskLabel => taskLabel.Project)
            .HasForeignKey(taskLabel => taskLabel.ProjectId)
            .IsRequired();

        builder.HasMany(project => project.Epics)
            .WithOne(epic => epic.Project)
            .HasForeignKey(epic => epic.ProjectId)
            .IsRequired();

        builder.HasMany(project => project.Sprints)
            .WithOne(sprint => sprint.Project)
            .HasForeignKey(sprint => sprint.ProjectId)
            .IsRequired();
        
        builder.HasMany(project => project.Tasks)
            .WithOne(task => task.Project)
            .HasForeignKey(task => task.ProjectId)
            .IsRequired();
    }
}
