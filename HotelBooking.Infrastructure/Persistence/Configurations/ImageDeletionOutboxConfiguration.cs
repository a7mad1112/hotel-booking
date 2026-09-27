using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class ImageDeletionOutboxConfiguration : IEntityTypeConfiguration<ImageDeletionOutbox>
{
    public void Configure(EntityTypeBuilder<ImageDeletionOutbox> builder)
    {
        builder.ToTable("image_deletion_outbox");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.LastError)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.CreatedAt);
    }
}
