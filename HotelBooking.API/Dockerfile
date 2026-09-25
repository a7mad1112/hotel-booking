# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files first for Docker layer caching
COPY ["HotelBooking.API/HotelBooking.API.csproj", "HotelBooking.API/"]
COPY ["HotelBooking.Application/HotelBooking.Application.csproj", "HotelBooking.Application/"]
COPY ["HotelBooking.Domain/HotelBooking.Domain.csproj", "HotelBooking.Domain/"]
COPY ["HotelBooking.Infrastructure/HotelBooking.Infrastructure.csproj", "HotelBooking.Infrastructure/"]

# Initial restore
RUN dotnet restore "HotelBooking.API/HotelBooking.API.csproj"

# Copy source
COPY . .

# Restore again after all source files are present
RUN dotnet restore "HotelBooking.API/HotelBooking.API.csproj" --force

WORKDIR "/src/HotelBooking.API"

# Build
RUN dotnet build "HotelBooking.API.csproj" \
    -c Release \
    -o /app/build \
    --no-restore

# Publish
RUN dotnet publish "HotelBooking.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HotelBooking.API.dll"]