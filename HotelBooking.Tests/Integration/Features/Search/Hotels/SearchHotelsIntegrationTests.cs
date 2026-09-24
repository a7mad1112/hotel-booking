using HotelBooking.Application.Features.Search.Hotels;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace HotelBooking.Tests.Integration.Features.Search.Hotels;

public class SearchHotelsIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("hotelbooking_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApplicationDbContext _context = null!;
    private HotelSearchRepository _repository = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);

        await _context.Database.EnsureCreatedAsync();

        _repository = new HotelSearchRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task SearchAsync_NoCriteria_ReturnsAllHotelsPaginated()
    {
        // Arrange
        var city = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        _context.Cities.Add(city);

        var owner = new User
        {
            Email = "owner@example.com",
            PasswordHash = "hash",
            Role = UserRole.Owner
        };

        _context.Users.Add(owner);

        await _context.SaveChangesAsync();

        var hotel1 = new Hotel
        {
            Name = "Grand Hotel",
            Description = "Grand hotel",
            StarRating = 5,
            Location = "Downtown",
            CityId = city.Id,
            OwnerId = owner.Id
        };

        var hotel2 = new Hotel
        {
            Name = "Royal Hotel",
            Description = "Royal hotel",
            StarRating = 4,
            Location = "City Center",
            CityId = city.Id,
            OwnerId = owner.Id
        };

        var hotel3 = new Hotel
        {
            Name = "Sunrise Hotel",
            Description = "Sunrise hotel",
            StarRating = 3,
            Location = "West Amman",
            CityId = city.Id,
            OwnerId = owner.Id
        };

        _context.Hotels.AddRange(
            hotel1,
            hotel2,
            hotel3);

        await _context.SaveChangesAsync();

        var request = new SearchHotelsRequest();

        // Act
        var result = await _repository.SearchAsync(
            request,
            page: 1,
            pageSize: 2,
            CancellationToken.None);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal(
            "Grand Hotel",
            result.Items[0].Name);

        Assert.Equal(
            "Royal Hotel",
            result.Items[1].Name);
    }

    [Fact]
    public async Task SearchAsync_WithDatesGuestsAndRooms_ExcludesHotelWithOverlappingBooking()
    {
        // Arrange
        var city = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        _context.Cities.Add(city);

        var owner = new User
        {
            Email = "owner@example.com",
            PasswordHash = "hash",
            Role = UserRole.Owner
        };

        var customer = new User
        {
            Email = "customer@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer
        };

        _context.Users.AddRange(
            owner,
            customer);

        var roomType = new RoomType
        {
            Name = "Standard",
            Description = "Standard room"
        };

        _context.RoomTypes.Add(roomType);

        await _context.SaveChangesAsync();

        var bookedHotel = new Hotel
        {
            Name = "Booked Hotel",
            Description = "Hotel with no available room",
            StarRating = 5,
            Location = "Downtown",
            CityId = city.Id,
            OwnerId = owner.Id
        };

        var availableHotel = new Hotel
        {
            Name = "Available Hotel",
            Description = "Hotel with available room",
            StarRating = 4,
            Location = "City Center",
            CityId = city.Id,
            OwnerId = owner.Id
        };

        _context.Hotels.AddRange(
            bookedHotel,
            availableHotel);

        await _context.SaveChangesAsync();

        var bookedRoom = new Room
        {
            HotelId = bookedHotel.Id,
            RoomNumber = "101",
            RoomTypeId = roomType.Id,
            PricePerNight = 100m,
            AdultsCapacity = 2,
            ChildrenCapacity = 1,
            Availability = true
        };

        var availableRoom = new Room
        {
            HotelId = availableHotel.Id,
            RoomNumber = "201",
            RoomTypeId = roomType.Id,
            PricePerNight = 150m,
            AdultsCapacity = 2,
            ChildrenCapacity = 1,
            Availability = true
        };

        _context.Rooms.AddRange(
            bookedRoom,
            availableRoom);

        await _context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = customer.Id,
            RoomId = bookedRoom.Id,
            CheckInDate = new DateTime(
                2026,
                10,
                10,
                0,
                0,
                0,
                DateTimeKind.Utc),
            CheckOutDate = new DateTime(
                2026,
                10,
                12,
                0,
                0,
                0,
                DateTimeKind.Utc),
            TotalPrice = 200m,
            Status = BookingStatus.Confirmed
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        var request = new SearchHotelsRequest
        {
            CheckInDate = new DateTime(
                2026,
                10,
                10,
                0,
                0,
                0,
                DateTimeKind.Utc),
            CheckOutDate = new DateTime(
                2026,
                10,
                12,
                0,
                0,
                0,
                DateTimeKind.Utc),
            Adults = 2,
            Children = 1,
            Rooms = 1
        };

        // Act
        var result = await _repository.SearchAsync(
            request,
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);

        var hotel = result.Items[0];

        Assert.Equal(
            "Available Hotel",
            hotel.Name);

        Assert.Equal(
            150m,
            hotel.PricePerNight);

        Assert.DoesNotContain(
            result.Items,
            x => x.Name == "Booked Hotel");
    }
}