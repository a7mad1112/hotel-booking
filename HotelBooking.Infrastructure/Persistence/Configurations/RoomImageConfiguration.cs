using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class RoomImageConfiguration
    : IEntityTypeConfiguration<RoomImage>
{
    public void Configure(EntityTypeBuilder<RoomImage> builder)
    {
        builder.ToTable("room_images");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageUrl)
            .IsRequired();

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}