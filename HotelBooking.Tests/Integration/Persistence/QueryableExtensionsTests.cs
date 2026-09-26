using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Tests.Integration.Persistence;

public class QueryableExtensionsTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private ApplicationDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        // Seed 5 test cities
        for (var i = 1; i <= 5; i++)
        {
            _context.Cities.Add(new City
            {
                Name = $"City_{i:D2}",
                Country = "Country"
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task ToPagedListAsync_FirstPage_ReturnsCorrectSubsetAndTotal()
    {
        // Act
        var (items, totalCount) = await _context.Cities
            .OrderBy(c => c.Name)
            .ToPagedListAsync(page: 1, pageSize: 2);

        // Assert
        Assert.Equal(5, totalCount);
        Assert.Equal(2, items.Count);
        Assert.Equal("City_01", items[0].Name);
        Assert.Equal("City_02", items[1].Name);
    }

    [Fact]
    public async Task ToPagedListAsync_SecondPage_ReturnsNextPageItems()
    {
        // Act
        var (items, totalCount) = await _context.Cities
            .OrderBy(c => c.Name)
            .ToPagedListAsync(page: 2, pageSize: 2);

        // Assert
        Assert.Equal(5, totalCount);
        Assert.Equal(2, items.Count);
        Assert.Equal("City_03", items[0].Name);
        Assert.Equal("City_04", items[1].Name);
    }

    [Fact]
    public async Task ToPagedListAsync_LastPage_ReturnsRemainingItems()
    {
        // Act
        var (items, totalCount) = await _context.Cities
            .OrderBy(c => c.Name)
            .ToPagedListAsync(page: 3, pageSize: 2);

        // Assert
        Assert.Equal(5, totalCount);
        Assert.Single(items);
        Assert.Equal("City_05", items[0].Name);
    }

    [Fact]
    public async Task ToPagedListAsync_InvalidPageLessThanOne_DefaultsToFirstPage()
    {
        // Act
        var (items, totalCount) = await _context.Cities
            .OrderBy(c => c.Name)
            .ToPagedListAsync(page: 0, pageSize: 2);

        // Assert
        Assert.Equal(5, totalCount);
        Assert.Equal(2, items.Count);
        Assert.Equal("City_01", items[0].Name);
    }

    [Fact]
    public async Task ToPagedListAsync_EmptyQuery_ReturnsZeroCountAndEmptyList()
    {
        // Act
        var (items, totalCount) = await _context.Cities
            .Where(c => c.Country == "NonExistent")
            .ToPagedListAsync(page: 1, pageSize: 10);

        // Assert
        Assert.Equal(0, totalCount);
        Assert.Empty(items);
    }
}
