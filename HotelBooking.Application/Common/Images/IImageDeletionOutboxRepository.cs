using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Common.Images;

public interface IImageDeletionOutboxRepository : IRepository<ImageDeletionOutbox>
{
    Task EnqueueRangeAsync(IEnumerable<string> publicIds, CancellationToken cancellationToken);

    Task<List<ImageDeletionOutbox>> GetPendingBatchAsync(int batchSize, int maxRetries, CancellationToken cancellationToken);

    void DeleteRange(IEnumerable<ImageDeletionOutbox> items);
}
