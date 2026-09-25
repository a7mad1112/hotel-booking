using HotelBooking.Application.Features.Search.Hotels;

namespace HotelBooking.Tests.Unit.Features.Search.Hotels;

public class SearchHotelsValidatorTests
{
    private readonly SearchHotelsValidator _validator = new();

    [Fact]
    public async Task Validate_NoCriteria_IsValid()
    {
        var request = new SearchHotelsRequest();
        var result = await _validator.ValidateAsync(request);
        Assert.True(result.IsValid);
    }


    [Fact]
    public async Task Validate_ValidCriteria_IsValid()
    {
        var request = new SearchHotelsRequest
        {
            CityId = 1,
            CheckInDate = DateTime.UtcNow.Date,
            CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
            Adults = 2,
            Children = 1,
            Rooms = 1,
            MinPrice = 50,
            MaxPrice = 200,
            MinStarRating = 3,
            MaxStarRating = 5,
            AmenityIds = [1, 2],
            RoomTypeId = 1
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }


    [Fact]
    public async Task Validate_InvalidCityId_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            CityId = 0
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_InvalidAdults_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            Adults = 0
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_InvalidChildren_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            Children = -1
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_InvalidRooms_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            Rooms = 0
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_CheckOutBeforeCheckIn_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            CheckInDate = new DateTime(2026, 10, 10),
            CheckOutDate = new DateTime(2026, 10, 5)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_MaxPriceLessThanMinPrice_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            MinPrice = 200,
            MaxPrice = 100
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }


    [Fact]
    public async Task Validate_MaxStarRatingLessThanMinStarRating_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            MinStarRating = 5,
            MaxStarRating = 3
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_SearchTermValid_IsValid()
    {
        var request = new SearchHotelsRequest
        {
            SearchTerm = "Grand Luxury Hotel"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_SearchTermTooLong_IsInvalid()
    {
        var request = new SearchHotelsRequest
        {
            SearchTerm = new string('A', 101)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}