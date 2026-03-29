using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadOrNot.Domain.Entities;

namespace ReadOrNot.Infrastructure.Persistence.Configurations;

internal sealed class OpenEventConfiguration : IEntityTypeConfiguration<OpenEvent>
{
    public void Configure(EntityTypeBuilder<OpenEvent> builder)
    {
        builder.ToTable("OpenEvents");

        builder.HasKey(openEvent => openEvent.Id);

        builder.Property(openEvent => openEvent.OccurredAtUtc)
            .IsRequired();

        builder.Property(openEvent => openEvent.IpAddress)
            .HasMaxLength(128);

        builder.Property(openEvent => openEvent.IpAddressHash)
            .HasMaxLength(128);

        builder.Property(openEvent => openEvent.UserAgent)
            .HasMaxLength(1024);

        builder.Property(openEvent => openEvent.Referer)
            .HasMaxLength(1024);

        builder.Property(openEvent => openEvent.AcceptLanguage)
            .HasMaxLength(256);

        builder.Property(openEvent => openEvent.QueryString)
            .HasMaxLength(2048);

        builder.Property(openEvent => openEvent.VisitorFingerprintHash)
            .HasMaxLength(128);

        builder.Property(openEvent => openEvent.BotSignals)
            .HasMaxLength(512);

        builder.Property(openEvent => openEvent.HttpMethod)
            .HasMaxLength(16);

        builder.Property(openEvent => openEvent.MetadataJson)
            .HasMaxLength(4000);

        builder.HasIndex(openEvent => openEvent.TokenId);
        builder.HasIndex(openEvent => openEvent.OccurredAtUtc);
        builder.HasIndex(openEvent => new { openEvent.TokenId, openEvent.OccurredAtUtc });
    }
}
