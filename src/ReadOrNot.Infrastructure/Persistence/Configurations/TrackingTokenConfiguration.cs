using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadOrNot.Domain.Entities;

namespace ReadOrNot.Infrastructure.Persistence.Configurations;

internal sealed class TrackingTokenConfiguration : IEntityTypeConfiguration<TrackingToken>
{
    public void Configure(EntityTypeBuilder<TrackingToken> builder)
    {
        builder.ToTable("TrackingTokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.PublicIdentifier)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(token => token.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(token => token.Description)
            .HasMaxLength(500);

        builder.Property(token => token.CreatedAtUtc)
            .IsRequired();

        builder.Property(token => token.IsEnabled)
            .IsRequired();

        builder.HasIndex(token => token.PublicIdentifier)
            .IsUnique();

        builder.HasIndex(token => new { token.UserId, token.Name });

        builder.HasMany(token => token.OpenEvents)
            .WithOne(openEvent => openEvent.Token)
            .HasForeignKey(openEvent => openEvent.TokenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
