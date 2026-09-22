using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class HotelAmenityConfiguration
    : IEntityTypeConfiguration<HotelAmenity>
{
    public void Configure(EntityTypeBuilder<HotelAmenity> builder)
    {
        builder.ToTable("hotel_amenities");

        builder.HasKey(x => new
        {
            x.HotelId,
            x.AmenityId
        });


        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.HotelAmenities)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.Amenity)
            .WithMany(x => x.HotelAmenities)
            .HasForeignKey(x => x.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}