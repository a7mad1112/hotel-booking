namespace HotelBooking.Application.Common.Images;

public interface IImageService
{
    Task<ImageUploadResult> UploadAsync(ImageUpload image, string folder, CancellationToken cancellationToken);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken);
}