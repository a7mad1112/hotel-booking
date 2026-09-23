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

    public async Task<List<GetBookingHistoryResponse>> GetBookingHistoryAsync(int userId, int count,
        CancellationToken cancellationToken)
    {
        var latestBookings = DbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Room.HotelId)
            .Select(group => group
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .First());

        return await latestBookings
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
    }
}