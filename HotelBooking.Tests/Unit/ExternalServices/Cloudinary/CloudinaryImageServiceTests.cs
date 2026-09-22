using HotelBooking.Application.Common.Images;
using HotelBooking.Infrastructure.ExternalServices.Cloudinary;
using Microsoft.Extensions.Options;
using Xunit;

namespace HotelBooking.Tests.Unit.ExternalServices.Cloudinary;

public class CloudinaryImageServiceTests
{
    [Fact]
    public async Task UploadAsync_EmptyImage_ThrowsArgumentException()
    {
        // Arrange
        var options = Options.Create(
            new CloudinaryOptions
            {
                CloudName = "test-cloud",
                ApiKey = "test-api-key",
                ApiSecret = "test-api-secret"
            });

        var service = new CloudinaryImageService(options);

        await using var stream = new MemoryStream();

        var image = new ImageUpload
        {
            Content = stream,
            FileName = "test.jpg"
        };

        // Act
        var act = () => service.UploadAsync(image, "test", CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }
}