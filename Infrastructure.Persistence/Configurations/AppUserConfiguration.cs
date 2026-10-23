using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(32);

        builder.Property(user => user.HashedPassword)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.PasswordSalt)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.LastLoginIp)
            .HasMaxLength(45);

        builder.Property(user => user.LastLoginDevice)
            .HasMaxLength(500);
    }
}
