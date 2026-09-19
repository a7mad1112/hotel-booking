using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class HotelConfiguration: IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("hotels");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.StarRating)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(500);
        
        builder.HasOne(x => x.City)
            .WithMany(x => x.Hotels)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Owner)
            .WithMany(x => x.Hotels)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
    }
}