using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class CustomerProductConfiguration : IEntityTypeConfiguration<CustomerProduct>
{
    public void Configure(EntityTypeBuilder<CustomerProduct> builder)
    {
        builder.HasKey(customerProduct => customerProduct.Id);

        builder.Property(customerProduct => customerProduct.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(customerProduct => customerProduct.StatusNote)
            .HasMaxLength(4_000);

        builder.HasIndex(customerProduct => new
            {
                customerProduct.CustomerId,
                customerProduct.ProductId
            })
            .IsUnique();

        builder.HasIndex(customerProduct => new
        {
            customerProduct.Status,
            customerProduct.NextFollowUpAt
        });

        builder.HasOne(customerProduct => customerProduct.Customer)
            .WithMany(customer => customer.CustomerProducts)
            .HasForeignKey(customerProduct => customerProduct.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(customerProduct => customerProduct.Product)
            .WithMany(product => product.CustomerProducts)
            .HasForeignKey(customerProduct => customerProduct.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
