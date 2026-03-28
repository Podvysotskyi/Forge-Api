using Forge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forge.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.HasIndex(user => user.Login).IsUnique();
        builder.HasIndex(user => user.Email).IsUnique();

        builder.HasOne(user => user.Status)
            .WithMany(userStatus => userStatus.Users)
            .HasForeignKey(user => user.StatusId);
    }
}
