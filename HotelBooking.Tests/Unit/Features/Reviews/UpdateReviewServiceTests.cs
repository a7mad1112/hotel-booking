using HotelBooking.Application.Features.Reviews;
using HotelBooking.Application.Features.Reviews.UpdateReview;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Reviews;

public class UpdateReviewServiceTests
{
    [Fact]
    public async Task UpdateAsync_Owner_ReturnsSuccess()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2, Rating = 3, Comment = "Okay" };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new UpdateReviewService(repository.Object);

        var request = new UpdateReviewRequest { Rating = 5, Comment = "Much better now!" };
        var result = await service.UpdateAsync(1, request, 10, false, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Rating);
        Assert.Equal("Much better now!", result.Value.Comment);

        repository.Verify(x => x.Update(existing), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Admin_ReturnsSuccess()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2, Rating = 3 };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new UpdateReviewService(repository.Object);

        var request = new UpdateReviewRequest { Rating = 4 };
        var result = await service.UpdateAsync(1, request, 999, true, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.Update(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UnauthorizedUser_ReturnsFailure()
    {
        var repository = new Mock<IReviewRepository>();
        var existing = new Review { Id = 1, UserId = 10, HotelId = 2, Rating = 3 };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new UpdateReviewService(repository.Object);

        var request = new UpdateReviewRequest { Rating = 1 };
        var result = await service.UpdateAsync(1, request, 999, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("You are not allowed to update this review.", result.Error);
        repository.Verify(x => x.Update(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReviewNotFound_ReturnsFailure()
    {
        var repository = new Mock<IReviewRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        var service = new UpdateReviewService(repository.Object);

        var request = new UpdateReviewRequest { Rating = 5 };
        var result = await service.UpdateAsync(99, request, 10, false, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Review not found.", result.Error);
    }
}
