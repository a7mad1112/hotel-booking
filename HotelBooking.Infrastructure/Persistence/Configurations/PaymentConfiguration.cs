using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.TransactionId)
            .IsUnique();

        builder.Property(x => x.Amount)
            .HasPrecision(12, 2);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Booking)
            .WithOne(x => x.Payment)
            .HasForeignKey<Payment>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}