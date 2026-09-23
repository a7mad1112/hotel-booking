using HotelBooking.Application.Common.Interfaces;

namespace HotelBooking.Application.Features.Users.GetBookingHistory;

public sealed class GetBookingHistoryService : IScopedService
{
    private const int HistoryCount = 5;

    private readonly IUserRepository _repository;

    public GetBookingHistoryService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetBookingHistoryResponse>> GetAsync(int userId, CancellationToken cancellationToken)
    {
        return await _repository.GetBookingHistoryAsync(userId, HistoryCount, cancellationToken);
    }
}