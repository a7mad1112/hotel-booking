using HotelBooking.Application.Common.Images;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HotelBooking.Tests.Unit.BackgroundJobs;

public class ImageCleanupBackgroundServiceTests
{
    [Fact]
    public async Task ProcessPendingDeletionsAsync_SuccessfulItems_DeletesFromCloudinaryAndRemovesFromOutbox()
    {
        // Arrange
        var outboxRepository = new Mock<IImageDeletionOutboxRepository>();
        var imageService = new Mock<IImageService>();

        var pendingItems = new List<ImageDeletionOutbox>
        {
            new() { Id = 1, PublicId = "hotel_img_1", RetryCount = 0 },
            new() { Id = 2, PublicId = "hotel_img_2", RetryCount = 0 }
        };

        outboxRepository
            .Setup(x => x.GetPendingBatchAsync(20, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingItems);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IImageDeletionOutboxRepository)))
            .Returns(outboxRepository.Object);
        serviceProvider
            .Setup(x => x.GetService(typeof(IImageService)))
            .Returns(imageService.Object);

        var scope = new Mock<IServiceScope>();
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

        var service = new ImageCleanupBackgroundService(
            scopeFactory.Object,
            NullLogger<ImageCleanupBackgroundService>.Instance);

        // Act
        var processedCount = await service.ProcessPendingDeletionsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, processedCount);

        imageService.Verify(
            x => x.DeleteAsync("hotel_img_1", It.IsAny<CancellationToken>()),
            Times.Once);

        imageService.Verify(
            x => x.DeleteAsync("hotel_img_2", It.IsAny<CancellationToken>()),
            Times.Once);

        outboxRepository.Verify(
            x => x.DeleteRange(It.Is<IEnumerable<ImageDeletionOutbox>>(items => items.Count() == 2)),
            Times.Once);

        outboxRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPendingDeletionsAsync_WhenCloudinaryFails_IncrementsRetryCountAndUpdatesOutbox()
    {
        // Arrange
        var outboxRepository = new Mock<IImageDeletionOutboxRepository>();
        var imageService = new Mock<IImageService>();

        var failedItem = new ImageDeletionOutbox
        {
            Id = 1,
            PublicId = "hotel_img_fail",
            RetryCount = 1
        };

        var pendingItems = new List<ImageDeletionOutbox> { failedItem };

        outboxRepository
            .Setup(x => x.GetPendingBatchAsync(20, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingItems);

        imageService
            .Setup(x => x.DeleteAsync("hotel_img_fail", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cloudinary API unavailable"));

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IImageDeletionOutboxRepository)))
            .Returns(outboxRepository.Object);
        serviceProvider
            .Setup(x => x.GetService(typeof(IImageService)))
            .Returns(imageService.Object);

        var scope = new Mock<IServiceScope>();
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

        var service = new ImageCleanupBackgroundService(
            scopeFactory.Object,
            NullLogger<ImageCleanupBackgroundService>.Instance);

        // Act
        var processedCount = await service.ProcessPendingDeletionsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, processedCount);
        Assert.Equal(2, failedItem.RetryCount);
        Assert.Equal("Cloudinary API unavailable", failedItem.LastError);

        outboxRepository.Verify(
            x => x.Update(failedItem),
            Times.Once);

        outboxRepository.Verify(
            x => x.DeleteRange(It.IsAny<IEnumerable<ImageDeletionOutbox>>()),
            Times.Never);

        outboxRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
