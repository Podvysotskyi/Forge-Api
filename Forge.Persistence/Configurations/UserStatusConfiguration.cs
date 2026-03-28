using Forge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forge.Persistence.Configurations;

internal sealed class UserStatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(userStatus => userStatus.Id);

        builder.HasIndex(userStatus => userStatus.Name).IsUnique();
    }
}
