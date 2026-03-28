using Forge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forge.Persistence.Configurations;

internal sealed class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
{
    public void Configure(EntityTypeBuilder<OrganizationUser> builder)
    {
        builder.HasKey(organizationUser => new { organizationUser.OrganizationId, organizationUser.UserId });

        builder.HasOne(organizationUser => organizationUser.Organization)
            .WithMany(organization => organization.OrganizationUsers)
            .HasForeignKey(organizationUser => organizationUser.OrganizationId);

        builder.HasOne(organizationUser => organizationUser.User)
            .WithMany(user => user.OrganizationUsers)
            .HasForeignKey(organizationUser => organizationUser.UserId);

        builder.HasOne(organizationUser => organizationUser.Status)
            .WithMany(userStatus => userStatus.OrganizationUsers)
            .HasForeignKey(organizationUser => organizationUser.StatusId)
            .IsRequired();

        builder.HasOne(organizationUser => organizationUser.Role)
            .WithMany()
            .HasForeignKey(organizationUser => organizationUser.RoleId)
            .IsRequired();

        builder.HasMany(organizationUser => organizationUser.Permissions)
            .WithMany(permission => permission.OrganizationUsers);
    }
}
