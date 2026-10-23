using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.HasKey(interaction => interaction.Id);

        builder.Property(interaction => interaction.Channel)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(interaction => interaction.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(interaction => interaction.Outcome)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(interaction => interaction.Notes)
            .IsRequired();

        builder.HasIndex(interaction => new
        {
            interaction.CustomerProductId,
            interaction.OccurredAt
        });

        builder.HasIndex(interaction => interaction.ScheduledAt);
        builder.HasIndex(interaction => interaction.NextFollowUpAt);

        builder.HasOne(interaction => interaction.CustomerProduct)
            .WithMany(customerProduct => customerProduct.Interactions)
            .HasForeignKey(interaction => interaction.CustomerProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
