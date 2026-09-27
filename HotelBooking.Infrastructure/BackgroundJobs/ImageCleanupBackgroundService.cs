using HotelBooking.Application.Common.Images;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure.BackgroundJobs;

public class ImageCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImageCleanupBackgroundService> _logger;
    private readonly TimeSpan _period;
    private const int BatchSize = 20;
    private const int MaxRetries = 5;

    public ImageCleanupBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ImageCleanupBackgroundService> logger,
        TimeSpan? period = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _period = period ?? TimeSpan.FromSeconds(30);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ImageCleanupBackgroundService started with poll period {Period}.", _period);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingDeletionsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ImageCleanupBackgroundService execution loop.");
            }

            try
            {
                await Task.Delay(_period, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("ImageCleanupBackgroundService stopped.");
    }

    public async Task<int> ProcessPendingDeletionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outboxRepo = scope.ServiceProvider.GetRequiredService<IImageDeletionOutboxRepository>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

        var pendingItems = await outboxRepo.GetPendingBatchAsync(BatchSize, MaxRetries, cancellationToken);

        if (pendingItems.Count == 0)
        {
            return 0;
        }

        _logger.LogInformation("Processing {Count} pending image deletion outbox items.", pendingItems.Count);

        var successfullyDeleted = new List<Domain.Entities.ImageDeletionOutbox>();

        foreach (var item in pendingItems)
        {
            try
            {
                await imageService.DeleteAsync(item.PublicId, cancellationToken);
                successfullyDeleted.Add(item);
                _logger.LogInformation("Successfully deleted image {PublicId} from Cloudinary.", item.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete image {PublicId} from Cloudinary. Attempt {Attempt}/{MaxRetries}",
                    item.PublicId, item.RetryCount + 1, MaxRetries);

                item.RetryCount++;
                item.LastError = ex.Message;
                outboxRepo.Update(item);
            }
        }

        if (successfullyDeleted.Count > 0)
        {
            outboxRepo.DeleteRange(successfullyDeleted);
        }

        await outboxRepo.SaveChangesAsync(cancellationToken);

        return successfullyDeleted.Count;
    }
}
