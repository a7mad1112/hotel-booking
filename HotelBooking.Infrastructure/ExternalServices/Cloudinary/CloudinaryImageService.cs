using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelBooking.Application.Common.Images;
using HotelBooking.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using CloudinaryClient = CloudinaryDotNet.Cloudinary;
using ImageUploadResult = HotelBooking.Application.Common.Images.ImageUploadResult;

namespace HotelBooking.Infrastructure.ExternalServices.Cloudinary;

public sealed class CloudinaryImageService : IImageService, IScopedService
{
    private readonly CloudinaryClient _cloudinary;

    public CloudinaryImageService(IOptions<CloudinaryOptions> options)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.CloudName))
        {
            throw new InvalidOperationException("Cloudinary cloud name is not configured.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("Cloudinary API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiSecret))
        {
            throw new InvalidOperationException("Cloudinary API secret is not configured.");
        }

        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);

        _cloudinary = new CloudinaryClient(account);

        _cloudinary.Api.Secure = true;
    }

    public async Task<ImageUploadResult> UploadAsync(ImageUpload image, string folder,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (image.Content.Length == 0)
        {
            throw new ArgumentException("Image content cannot be empty.",
                nameof(image));
        }

        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new ArgumentException("Image folder cannot be empty.", nameof(folder));
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, image.Content),
            Folder = folder
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary image upload failed: {result.Error.Message}");
        }

        if (string.IsNullOrWhiteSpace(result.PublicId))
        {
            throw new InvalidOperationException("Cloudinary upload succeeded but did not return a public ID.");
        }

        var url = result.SecureUrl?.ToString()
                  ?? result.Url?.ToString();

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException("Cloudinary upload succeeded but did not return an image URL.");
        }

        return new ImageUploadResult
        {
            Url = url,
            PublicId = result.PublicId
        };
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            throw new ArgumentException("Public ID cannot be empty.", nameof(publicId));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var deleteParams = new DeletionParams(publicId)
        {
            Invalidate = true
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary image deletion failed: {result.Error.Message}");
        }
    }
}