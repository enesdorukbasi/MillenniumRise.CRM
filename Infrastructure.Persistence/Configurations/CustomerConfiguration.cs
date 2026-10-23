using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.CompanyName)
            .HasMaxLength(200);

        builder.Property(customer => customer.PhoneNumber)
            .HasMaxLength(32);

        builder.Property(customer => customer.Email)
            .HasMaxLength(320);

        builder.Property(customer => customer.CustomerType)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(customer => customer.Sector)
            .HasMaxLength(150);

        builder.Property(customer => customer.Website)
            .HasMaxLength(2_048);

        builder.Property(customer => customer.SourceUrl)
            .HasMaxLength(2_048);

        builder.Property(customer => customer.Location)
            .HasMaxLength(300);

        builder.Property(customer => customer.Note)
            .HasMaxLength(4_000);

        builder.HasIndex(customer => customer.FullName);
        builder.HasIndex(customer => customer.CompanyName);
        builder.HasIndex(customer => customer.Email);
        builder.HasIndex(customer => customer.PhoneNumber);
    }
}
