using FluentValidation;

namespace HotelBooking.API.Features.Hotels.UploadHotelImage;

public sealed class UploadHotelImageValidator
    : AbstractValidator<UploadHotelImageRequest>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public UploadHotelImageValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Image is required.");

        RuleFor(x => x.Image)
            .Must(image => image is not null && image.Length > 0)
            .WithMessage("Image cannot be empty.");

        RuleFor(x => x.Image)
            .Must(image => image is not null && image.Length <= MaxFileSize)
            .WithMessage("Image size cannot exceed 5 MB.");

        RuleFor(x => x.Image)
            .Must(image => image is not null && AllowedContentTypes.Contains(
                image.ContentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage(
                "Only JPEG, PNG, and WebP images are allowed.");
    }
}