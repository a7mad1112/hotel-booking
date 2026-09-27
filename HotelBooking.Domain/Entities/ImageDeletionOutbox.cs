using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class ImageDeletionOutbox : BaseEntity
{
    public required string PublicId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public int RetryCount { get; set; } = 0;
    public string? LastError { get; set; }
}
