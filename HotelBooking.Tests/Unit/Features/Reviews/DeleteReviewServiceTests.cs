using HotelBooking.Application.Features.Reviews;
using HotelBooking.Application.Features.Reviews.DeleteReview;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Reviews;

public class DeleteReviewServiceTests
{
    [Fact]
    public async Task DeleteAsync_Owner_ReturnsSuccess()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2 };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new DeleteReviewService(repository.Object);

        var result = await service.DeleteAsync(1, 10, false, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.Delete(existing), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Admin_ReturnsSuccess()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2 };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new DeleteReviewService(repository.Object);

        var result = await service.DeleteAsync(1, 999, true, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.Delete(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UnauthorizedUser_ReturnsFailure()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2 };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new DeleteReviewService(repository.Object);

        var result = await service.DeleteAsync(1, 999, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("You are not allowed to delete this review.", result.Error);
        repository.Verify(x => x.Delete(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReviewNotFound_ReturnsFailure()
    {
        var repository = new Mock<IReviewRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        var service = new DeleteReviewService(repository.Object);

        var result = await service.DeleteAsync(99, 10, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Review not found.", result.Error);
    }
}
