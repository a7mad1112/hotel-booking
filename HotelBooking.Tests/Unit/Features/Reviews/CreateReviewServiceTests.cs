using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Reviews;
using HotelBooking.Application.Features.Reviews.CreateReview;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Reviews;

public class CreateReviewServiceTests
{
    [Fact]
    public async Task CreateAsync_UserHasBooked_ReturnsSuccess()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var hotelRepo = new Mock<IHotelRepository>();

        hotelRepo
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Hotel { Id = 1, Name = "Grand Hotel" });

        reviewRepo
            .Setup(x => x.HasUserBookedHotelAsync(10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CreateReviewService(reviewRepo.Object, hotelRepo.Object);

        var request = new CreateReviewRequest
        {
            Rating = 5,
            Comment = "Excellent stay!"
        };

        var result = await service.CreateAsync(1, request, 10, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Rating);
        Assert.Equal("Excellent stay!", result.Value.Comment);
        Assert.Equal(1, result.Value.HotelId);
        Assert.Equal(10, result.Value.UserId);

        reviewRepo.Verify(x => x.AddAsync(It.Is<Review>(r => r.Rating == 5 && r.HotelId == 1 && r.UserId == 10), It.IsAny<CancellationToken>()), Times.Once);
        reviewRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_HotelNotFound_ReturnsFailure()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var hotelRepo = new Mock<IHotelRepository>();

        hotelRepo
            .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);

        var service = new CreateReviewService(reviewRepo.Object, hotelRepo.Object);

        var request = new CreateReviewRequest { Rating = 4 };

        var result = await service.CreateAsync(99, request, 10, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Hotel not found.", result.Error);
        reviewRepo.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserHasNotBooked_ReturnsFailure()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var hotelRepo = new Mock<IHotelRepository>();

        hotelRepo
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Hotel { Id = 1, Name = "Grand Hotel" });

        reviewRepo
            .Setup(x => x.HasUserBookedHotelAsync(10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CreateReviewService(reviewRepo.Object, hotelRepo.Object);

        var request = new CreateReviewRequest { Rating = 5 };

        var result = await service.CreateAsync(1, request, 10, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Only guests who have booked this hotel can submit a review.", result.Error);
        reviewRepo.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
