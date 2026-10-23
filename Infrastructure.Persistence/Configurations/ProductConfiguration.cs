using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(4_000);

        builder.HasIndex(product => product.Name)
            .IsUnique();

        builder.HasIndex(product => product.IsActive);

        builder.HasOne(product => product.ProductLogo)
            .WithMany()
            .HasForeignKey(product => product.ProductLogoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
