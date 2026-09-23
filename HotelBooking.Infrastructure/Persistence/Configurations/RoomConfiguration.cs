using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Configurations.Extensions;


namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoomNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.PricePerNight)
            .IsRequired()
            .HasPrecision(12, 2);

        builder.HasIndex(x => new
        {
            x.HotelId,
            x.RoomNumber
        }).IsUnique();


        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RoomType)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditProperties();
    }
}