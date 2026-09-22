using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class City : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    public ICollection<Hotel> Hotels { get; set; } = [];
}