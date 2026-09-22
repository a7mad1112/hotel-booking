using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Configurations.Extensions;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();


        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Hotel)
            .WithMany(h => h.Reviews)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditProperties();
    }
}