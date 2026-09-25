using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Amenities;
using HotelBooking.Application.Features.Amenities.GetAmenities;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Amenities;

public class GetAmenitiesServiceTests
{
    [Fact]
    public async Task GetAllAsync_Paged_ReturnsPagedResult()
    {
        var repository = new Mock<IAmenityRepository>();
        var items = new List<Amenity>
        {
            new() { Id = 1, Name = "Free Parking" },
            new() { Id = 2, Name = "Pool" }
        };

        repository
            .Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 2));

        var service = new GetAmenitiesService(repository.Object);

        var request = new PaginationRequest { Page = 1, PageSize = 10 };
        var result = await service.GetAllAsync(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Free Parking", result.Items[0].Name);
        Assert.Equal("Pool", result.Items[1].Name);
    }
}
