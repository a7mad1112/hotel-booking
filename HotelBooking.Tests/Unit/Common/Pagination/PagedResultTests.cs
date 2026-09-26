using HotelBooking.Application.Common.Pagination;

namespace HotelBooking.Tests.Unit.Common.Pagination;

public class PagedResultTests
{
    [Fact]
    public void Create_WithValidParameters_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var result = PagedResult<string>.Create(items, totalCount: 15, page: 2, pageSize: 5);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("item1", result.Items[0]);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public void Create_WithPaginationRequest_UsesRequestPageAndPageSize()
    {
        // Arrange
        var items = new List<int> { 1, 2 };
        var request = new PaginationRequest { Page = 3, PageSize = 20 };

        // Act
        var result = PagedResult<int>.Create(items, totalCount: 45, request);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(45, result.TotalCount);
        Assert.Equal(3, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(3, result.TotalPages); // ceil(45/20) = 3
    }

    [Fact]
    public void Create_WithNullItems_InitializesEmptyList()
    {
        // Act
        var result = PagedResult<string>.Create(null!, totalCount: 0, page: 1, pageSize: 10);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void Create_WithZeroOrNegativePageAndSize_NormalizesValues()
    {
        // Act
        var result = PagedResult<string>.Create(new List<string>(), totalCount: 0, page: -1, pageSize: 0);

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public void Create_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PagedResult<string>.Create(new List<string>(), totalCount: 0, request: null!));
    }
}
