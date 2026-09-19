using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
{
    public void Configure(EntityTypeBuilder<HotelImage> builder)
    {
        builder.ToTable("hotel_images");

        builder.HasKey(I => I.Id);

        builder.Property(I => I.ImageUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(I => I.Hotel)
            .WithMany(I => I.Images)
            .HasForeignKey(I => I.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.PublicId)
            .IsRequired()
            .HasMaxLength(500);
    }
}