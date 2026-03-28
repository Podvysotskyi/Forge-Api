using Forge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forge.Persistence.Configurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(organization => organization.Id);

        builder.HasIndex(organization => organization.Name).IsUnique();

        builder.HasOne(organization => organization.User)
            .WithOne(user => user.Organization)
            .HasForeignKey<Organization>(organization => organization.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
