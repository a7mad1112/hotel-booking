using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Authentication.Login;

public interface IJwtTokenGenerator
{
    JwtTokenResult Generate(User user);
}