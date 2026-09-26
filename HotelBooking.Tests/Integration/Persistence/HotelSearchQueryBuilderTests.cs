using HotelBooking.Application.Features.Search.Hotels;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Repositories.Search;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace HotelBooking.Tests.Integration.Persistence;

public class HotelSearchQueryBuilderTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("hotelbooking_test_search")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApplicationDbContext _context = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        var cityAmman = new City { Name = "Amman", Country = "Jordan" };
        var cityAqaba = new City { Name = "Aqaba", Country = "Jordan" };
        _context.Cities.AddRange(cityAmman, cityAqaba);

        var owner = new User
        {
            Email = "owner@hotel.com",
            PasswordHash = "hash",
            Role = Domain.Enums.UserRole.Owner
        };
        _context.Users.Add(owner);
        await _context.SaveChangesAsync();

        var hotel1 = new Hotel
        {
            Name = "Grand Amman",
            Description = "Luxury stay",
            Location = "Downtown",
            StarRating = 5,
            CityId = cityAmman.Id,
            OwnerId = owner.Id
        };

        var hotel2 = new Hotel
        {
            Name = "Red Sea Resort",
            Description = "Beach stay",
            Location = "Coastal",
            StarRating = 4,
            CityId = cityAqaba.Id,
            OwnerId = owner.Id
        };

        _context.Hotels.AddRange(hotel1, hotel2);
        await _context.SaveChangesAsync();

        var roomType = new RoomType { Name = "Deluxe" };
        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync();

        var room1 = new Room
        {
            HotelId = hotel1.Id,
            RoomTypeId = roomType.Id,
            RoomNumber = "101",
            PricePerNight = 150,
            AdultsCapacity = 2,
            ChildrenCapacity = 1,
            Availability = true
        };

        var room2 = new Room
        {
            HotelId = hotel2.Id,
            RoomTypeId = roomType.Id,
            RoomNumber = "201",
            PricePerNight = 250,
            AdultsCapacity = 4,
            ChildrenCapacity = 2,
            Availability = true
        };

        _context.Rooms.AddRange(room1, room2);
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task WithSearchTerm_MatchesName()
    {
        // Act
        var query = HotelSearchQueryBuilder
            .Create(_context.Hotels.Include(h => h.City))
            .WithSearchTerm("Grand")
            .Build();

        var result = await query.ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Grand Amman", result[0].Name);
    }

    [Fact]
    public async Task WithCity_FiltersCorrectly()
    {
        // Arrange
        var aqaba = await _context.Cities.FirstAsync(c => c.Name == "Aqaba");

        // Act
        var query = HotelSearchQueryBuilder
            .Create(_context.Hotels)
            .WithCity(aqaba.Id)
            .Build();

        var result = await query.ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Red Sea Resort", result[0].Name);
    }

    [Fact]
    public async Task WithStarRating_FiltersMinAndMax()
    {
        // Act
        var query = HotelSearchQueryBuilder
            .Create(_context.Hotels)
            .WithStarRating(minStarRating: 5, maxStarRating: 5)
            .Build();

        var result = await query.ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(5, result[0].StarRating);
    }

    [Fact]
    public async Task WithRoomCriteria_FiltersByPrice()
    {
        // Arrange
        var request = new SearchHotelsRequest
        {
            MinPrice = 200
        };

        // Act
        var query = HotelSearchQueryBuilder
            .Create(_context.Hotels)
            .WithRoomCriteria(request)
            .Build();

        var result = await query.ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Red Sea Resort", result[0].Name);
    }

    [Fact]
    public async Task ProjectToResponse_ProjectsFieldsCorrectly()
    {
        // Arrange
        var request = new SearchHotelsRequest();

        // Act
        var items = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Images)
            .Include(h => h.Rooms)
            .Where(h => h.Name == "Grand Amman")
            .Select(HotelSearchQueryBuilder.ProjectToResponse(request))
            .ToListAsync();

        // Assert
        Assert.Single(items);
        Assert.Equal("Grand Amman", items[0].Name);
        Assert.Equal("Amman", items[0].CityName);
        Assert.Equal(150, items[0].PricePerNight);
    }
}
