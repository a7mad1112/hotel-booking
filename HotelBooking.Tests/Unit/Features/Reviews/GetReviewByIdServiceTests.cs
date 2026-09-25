using HotelBooking.Application.Features.Reviews;
using HotelBooking.Application.Features.Reviews.GetReviewById;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Reviews;

public class GetReviewByIdServiceTests
{
    [Fact]
    public async Task GetAsync_ExistingReview_ReturnsSuccess()
    {
        var repository = new Mock<IReviewRepository>();
        var user = new User { Id = 10, Email = "guest@example.com" };
        var review = new Review
        {
            Id = 1,
            UserId = 10,
            User = user,
            HotelId = 2,
            Rating = 5,
            Comment = "Awesome!"
        };

        repository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        var service = new GetReviewByIdService(repository.Object);

        var result = await service.GetAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("guest@example.com", result.Value.UserEmail);
        Assert.Equal(5, result.Value.Rating);
        Assert.Equal("Awesome!", result.Value.Comment);
    }

    [Fact]
    public async Task GetAsync_ReviewNotFound_ReturnsFailure()
    {
        var repository = new Mock<IReviewRepository>();
        repository
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        var service = new GetReviewByIdService(repository.Object);

        var result = await service.GetAsync(99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Review not found.", result.Error);
    }
}
