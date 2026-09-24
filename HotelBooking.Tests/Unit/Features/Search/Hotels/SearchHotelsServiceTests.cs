using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Search;
using HotelBooking.Application.Features.Search.Hotels;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Search.Hotels;

public class SearchHotelsServiceTests
{
    [Fact]
    public async Task SearchAsync_NoFilters_ReturnsPagedHotels()
    {
        var repository = new Mock<IHotelSearchRepository>();

        var hotels = new List<SearchHotelsResponse>
        {
            new()
            {
                Id = 1,
                Name = "Grand Hotel",
                Description = "A nice hotel",
                StarRating = 5,
                Location = "Downtown",
                CityId = 1,
                CityName = "Amman",
                ThumbnailUrl = "hotel.jpg",
                PricePerNight = 100
            }
        };

        repository
            .Setup(x => x.SearchAsync(
                It.IsAny<SearchHotelsRequest>(),
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((hotels, 1));

        var service = new SearchHotelsService(repository.Object);

        var request = new SearchHotelsRequest();

        var pagination = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };

        var result = await service.SearchAsync(request, pagination, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);

        Assert.Single(result.Items);

        Assert.Equal("Grand Hotel", result.Items[0].Name);

        Assert.Equal(100, result.Items[0].PricePerNight);
    }


    [Fact]
    public async Task SearchAsync_WithFilters_PassesFiltersToRepository()
    {
        var repository = new Mock<IHotelSearchRepository>();

        repository
            .Setup(x => x.SearchAsync(
                It.IsAny<SearchHotelsRequest>(),
                2,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (new List<SearchHotelsResponse>(), 0));

        var service = new SearchHotelsService(repository.Object);

        var request = new SearchHotelsRequest
        {
            CityId = 3,
            CheckInDate = new DateTime(2026, 10, 1),
            CheckOutDate = new DateTime(2026, 10, 5),
            Adults = 2,
            Children = 1,
            Rooms = 2,
            MinPrice = 50,
            MaxPrice = 200,
            MinStarRating = 4,
            MaxStarRating = 5,
            AmenityIds = [1, 2],
            RoomTypeId = 3
        };

        var pagination = new PaginationRequest
        {
            Page = 2,
            PageSize = 20
        };

        await service.SearchAsync(request, pagination, CancellationToken.None);

        repository.Verify(
            x => x.SearchAsync(
                It.Is<SearchHotelsRequest>(r =>
                    r.CityId == 3 &&
                    r.CheckInDate == new DateTime(2026, 10, 1) &&
                    r.CheckOutDate == new DateTime(2026, 10, 5) &&
                    r.Adults == 2 &&
                    r.Children == 1 &&
                    r.Rooms == 2 &&
                    r.MinPrice == 50 &&
                    r.MaxPrice == 200 &&
                    r.MinStarRating == 4 &&
                    r.MaxStarRating == 5 &&
                    r.RoomTypeId == 3 &&
                    r.AmenityIds.Count == 2 &&
                    r.AmenityIds.Contains(1) &&
                    r.AmenityIds.Contains(2)),
                2,
                20,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task SearchAsync_EmptyResult_ReturnsEmptyPagedResult()
    {
        var repository = new Mock<IHotelSearchRepository>();

        repository
            .Setup(x => x.SearchAsync(
                It.IsAny<SearchHotelsRequest>(),
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (new List<SearchHotelsResponse>(), 0));

        var service = new SearchHotelsService(repository.Object);

        var result =
            await service.SearchAsync(new SearchHotelsRequest(), new PaginationRequest(), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }
}