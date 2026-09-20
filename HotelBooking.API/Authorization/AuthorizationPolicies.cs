namespace HotelBooking.API.Authorization;

public static class AuthorizationPolicies
{
    public const string ManageCities = "ManageCities";
    public const string ManageHotels = "ManageHotels";
    public const string ManageRooms = "ManageRooms";
    public const string CreateBooking = "CreateBooking";
    public const string ViewHotels = "ViewHotels";
}