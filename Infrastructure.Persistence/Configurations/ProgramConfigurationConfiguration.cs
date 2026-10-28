using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ProgramConfigurationConfiguration : IEntityTypeConfiguration<ProgramConfiguration>
{
    public void Configure(EntityTypeBuilder<ProgramConfiguration> builder)
    {
        builder.HasKey(configuration => configuration.Id);

        builder.Property(configuration => configuration.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(configuration => configuration.Logo)
            .WithMany()
            .HasForeignKey(configuration => configuration.LogoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(configuration => configuration.AuthPageImage)
            .WithMany()
            .HasForeignKey(configuration => configuration.AuthPageImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
