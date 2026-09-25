using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Reviews;
using HotelBooking.Application.Features.Reviews.GetHotelReviews;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Reviews;

public class GetHotelReviewsServiceTests
{
    [Fact]
    public async Task GetByHotelIdAsync_ReturnsPagedResult()
    {
        var repository = new Mock<IReviewRepository>();
        var user = new User { Id = 10, Email = "guest@example.com" };
        var reviews = new List<Review>
        {
            new() { Id = 1, UserId = 10, User = user, HotelId = 2, Rating = 5, Comment = "Great!" },
            new() { Id = 2, UserId = 10, User = user, HotelId = 2, Rating = 4, Comment = "Good" }
        };

        repository
            .Setup(x => x.GetPagedByHotelIdAsync(2, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 2));

        var service = new GetHotelReviewsService(repository.Object);

        var request = new PaginationRequest { Page = 1, PageSize = 10 };
        var result = await service.GetByHotelIdAsync(2, request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("guest@example.com", result.Items[0].UserEmail);
        Assert.Equal(5, result.Items[0].Rating);
    }
}
