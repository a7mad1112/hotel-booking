using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Application.Features.Users;
using HotelBooking.Application.Features.Users.GetBookingHistory;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository, IScopedService
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<GetBookingHistoryResponse>> GetBookingHistoryAsync(int userId, int count, CancellationToken cancellationToken)
    {
        // First determine the latest booking for each hotel.
        // Only booking IDs are selected here to keep the GroupBy query
        // simple and avoid EF Core projection issues.
        var latestBookingIds = await DbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Room.HotelId)
            .Select(group => group
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Select(x => x.Id)
                .First())
            .ToListAsync(cancellationToken);

        if (latestBookingIds.Count == 0)
        {
            return [];
        }

        // Load the actual booking information separately.
        var history = await DbContext.Bookings
            .AsNoTracking()
            .Where(x => latestBookingIds.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(count)
            .Select(x => new GetBookingHistoryResponse
            {
                HotelId = x.Room.Hotel.Id,
                HotelName = x.Room.Hotel.Name,
                CityName = x.Room.Hotel.City.Name,
                StarRating = x.Room.Hotel.StarRating,
                ImageUrl = x.Room.Hotel.Images
                    .OrderBy(image => image.Id)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault(),
                PricePerNight = x.Room.PricePerNight
            })
            .ToListAsync(cancellationToken);

        return history;
    }
}