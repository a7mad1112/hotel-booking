using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence.Configurations.Extensions;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("deals");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.HasOne(d => d.Hotel)
            .WithMany(h => h.Deals)
            .HasForeignKey(d => d.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureAuditProperties();
    }
}