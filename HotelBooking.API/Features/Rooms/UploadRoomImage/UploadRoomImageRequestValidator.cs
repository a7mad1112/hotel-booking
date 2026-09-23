using FluentValidation;

namespace HotelBooking.API.Features.Rooms.UploadRoomImage;

public sealed class UploadRoomImageRequestValidator : AbstractValidator<UploadRoomImageRequest>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;

    public UploadRoomImageRequestValidator()
    {
        RuleFor(x => x.Image)
            .NotNull();

        RuleFor(x => x.Image)
            .Must(file =>
                file is not null && file.Length > 0)
            .WithMessage("Image cannot be empty.");

        RuleFor(x => x.Image)
            .Must(file => file is null || AllowedContentTypes.Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("Only JPEG, PNG, and WebP images are allowed.");

        RuleFor(x => x.Image)
            .Must(file => file is null || file.Length <= MaxFileSize)
            .WithMessage("Image size cannot exceed 5 MB.");
    }
}