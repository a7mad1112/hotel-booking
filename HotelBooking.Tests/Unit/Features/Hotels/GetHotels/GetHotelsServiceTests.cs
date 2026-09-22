using HotelBooking.Application.Common.Pagination;
using HotelBooking.Application.Features.Hotels;
using HotelBooking.Application.Features.Hotels.GetHotels;
using HotelBooking.Domain.Entities;
using Moq;

namespace HotelBooking.Tests.Unit.Features.Hotels.GetHotels;

public class GetHotelsServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsHotels()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new List<Hotel>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "Grand Hotel",
                            Description = "A nice hotel",
                            StarRating = 5,
                            Location = "Downtown",
                            CityId = 1,
                            City = new City
                            {
                                Id = 1,
                                Name = "Amman",
                                Country = "Jordan"
                            },
                            OwnerId = 2,
                            Owner = new User
                            {
                                Id = 2,
                                Email = "owner@example.com"
                            }
                        }
                    },
                    1
                ));


        var service = new GetHotelsService(
            repository.Object);


        var request = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };


        var result =
            await service.GetAllAsync(
                request,
                CancellationToken.None);


        Assert.Single(result.Items);

        var hotel = result.Items[0];

        Assert.Equal(
            "Grand Hotel",
            hotel.Name);

        Assert.Equal(
            "Amman",
            hotel.CityName);

        Assert.Equal(
            "owner@example.com",
            hotel.OwnerEmail);

        Assert.Equal(
            5,
            hotel.StarRating);

        Assert.Equal(
            1,
            result.TotalCount);
    }


    [Fact]
    public async Task GetAllAsync_NoHotels_ReturnsEmptyResult()
    {
        var repository = new Mock<IHotelRepository>();

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new List<Hotel>(),
                    0
                ));


        var service = new GetHotelsService(
            repository.Object);


        var request = new PaginationRequest
        {
            Page = 1,
            PageSize = 10
        };


        var result =
            await service.GetAllAsync(
                request,
                CancellationToken.None);


        Assert.Empty(result.Items);

        Assert.Equal(
            0,
            result.TotalCount);
    }
}