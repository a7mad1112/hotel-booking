using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class ImageDeletionOutboxRepository : Repository<ImageDeletionOutbox>, IImageDeletionOutboxRepository, IScopedService
{
    public ImageDeletionOutboxRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task EnqueueRangeAsync(IEnumerable<string> publicIds, CancellationToken cancellationToken)
    {
        var items = publicIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .Select(id => new ImageDeletionOutbox
            {
                PublicId = id,
                CreatedAt = DateTimeOffset.UtcNow,
                RetryCount = 0
            })
            .ToList();

        if (items.Count > 0)
        {
            await DbContext.ImageDeletionOutboxes.AddRangeAsync(items, cancellationToken);
        }
    }

    public async Task<List<ImageDeletionOutbox>> GetPendingBatchAsync(int batchSize, int maxRetries, CancellationToken cancellationToken)
    {
        return await DbContext.ImageDeletionOutboxes
            .Where(x => x.RetryCount < maxRetries)
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public void DeleteRange(IEnumerable<ImageDeletionOutbox> items)
    {
        DbContext.ImageDeletionOutboxes.RemoveRange(items);
    }
}
