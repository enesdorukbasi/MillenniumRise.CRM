using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.HasKey(file => file.Id);

        builder.Property(file => file.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(file => file.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(file => file.Extension)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(file => file.Url)
            .HasMaxLength(2_048)
            .IsRequired();

        builder.Property(file => file.ContentType)
            .HasMaxLength(255)
            .IsRequired();
    }
}
