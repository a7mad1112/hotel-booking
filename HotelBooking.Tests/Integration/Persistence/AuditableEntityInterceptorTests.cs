using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Tests.Integration.Persistence;

public class AuditableEntityInterceptorTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private ApplicationDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(new AuditableEntityInterceptor())
            .Options;

        _context = new ApplicationDbContext(options);

        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_NewEntity_SetsCreatedAndUpdatedAt()
    {
        // Arrange
        var city = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        // Act
        _context.Cities.Add(city);

        await _context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, city.CreatedAt);
        Assert.NotEqual(default, city.UpdatedAt);
        Assert.Equal(city.CreatedAt, city.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedEntity_UpdatesUpdatedAt()
    {
        // Arrange
        var city = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        _context.Cities.Add(city);

        await _context.SaveChangesAsync();

        var createdAt = city.CreatedAt;
        var originalUpdatedAt = city.UpdatedAt;

        while (DateTimeOffset.UtcNow <= originalUpdatedAt)
        {
            await Task.Delay(1);
        }

        // Act
        city.Name = "Amman City";

        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(createdAt, city.CreatedAt);
        Assert.True(city.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_UnchangedEntity_DoesNotUpdateUpdatedAt()
    {
        // Arrange
        var city = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        _context.Cities.Add(city);

        await _context.SaveChangesAsync();

        var originalUpdatedAt = city.UpdatedAt;

        await Task.Delay(10);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(originalUpdatedAt, city.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_MultipleEntities_SetsTimestampsForAll()
    {
        // Arrange
        var city1 = new City
        {
            Name = "Amman",
            Country = "Jordan"
        };

        var city2 = new City
        {
            Name = "Dubai",
            Country = "UAE"
        };

        // Act
        _context.Cities.AddRange(city1, city2);

        await _context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, city1.CreatedAt);
        Assert.NotEqual(default, city1.UpdatedAt);

        Assert.NotEqual(default, city2.CreatedAt);
        Assert.NotEqual(default, city2.UpdatedAt);
    }
}