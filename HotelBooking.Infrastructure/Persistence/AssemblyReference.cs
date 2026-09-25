using System.Reflection;

namespace HotelBooking.Infrastructure.Persistence;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}