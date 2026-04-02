using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain.Entities;

namespace TaskForge.Persistence.Configurations;

internal sealed class EpicConfiguration : IEntityTypeConfiguration<Epic>
{
    public void Configure(EntityTypeBuilder<Epic> builder)
    {
        builder.HasKey(epic => epic.Id);

        builder.Property(epic => epic.Name)
            .IsRequired()
            .HasMaxLength(45);

        builder.HasIndex(epic => new { epic.ProjectId, epic.Name })
            .IsUnique();

        builder.HasOne(epic => epic.Project)
            .WithMany(project => project.Epics)
            .HasForeignKey(epic => epic.ProjectId)
            .IsRequired();

        builder.HasMany(epic => epic.Tasks)
            .WithMany(task => task.Epics);
    }
}
