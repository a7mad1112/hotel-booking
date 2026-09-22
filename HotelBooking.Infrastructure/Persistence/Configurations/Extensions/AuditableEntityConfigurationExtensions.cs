using HotelBooking.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations.Extensions;

public static class AuditableEntityConfigurationExtensions
{
    public static void ConfigureAuditProperties<T>(
        this EntityTypeBuilder<T> builder)
        where T : class, IAuditableEntity
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}