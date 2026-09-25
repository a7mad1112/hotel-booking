using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken = default)
    {
        // ============================================================
        // 0. Apply database migrations
        // ============================================================

        await context.Database.MigrateAsync(cancellationToken);

        // ============================================================
        // 1. Only seed a completely empty database
        // ============================================================

        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        // ============================================================
        // 2. USERS
        // ============================================================

        var users = new List<User>();

        // ------------------------------------------------------------
        // Admin
        // ------------------------------------------------------------

        var admin = CreateUser(
            email: "admin@hotelbooking.com",
            password: "Admin123!",
            role: UserRole.Admin,
            passwordHasher);

        users.Add(admin);

        // ------------------------------------------------------------
        // Owners
        // ------------------------------------------------------------

        for (var i = 1; i <= 25; i++)
        {
            var owner = CreateUser(
                email: $"owner{i}@hotelbooking.com",
                password: "Owner123!",
                role: UserRole.Owner,
                passwordHasher);

            users.Add(owner);
        }

        // ------------------------------------------------------------
        // Customers
        // ------------------------------------------------------------

        for (var i = 1; i <= 250; i++)
        {
            var customer = CreateUser(
                email: $"customer{i}@hotelbooking.com",
                password: "Customer123!",
                role: UserRole.Customer,
                passwordHasher);

            users.Add(customer);
        }

        await context.Users.AddRangeAsync(users, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var owners = users
            .Where(x => x.Role == UserRole.Owner)
            .ToList();

        var customers = users
            .Where(x => x.Role == UserRole.Customer)
            .ToList();

        // ============================================================
        // 3. CITIES
        // ============================================================

        var cities = new List<City>
        {
            new()
            {
                Name = "Amman",
                Country = "Jordan",
                PostalCode = "11118"
            },
            new()
            {
                Name = "Aqaba",
                Country = "Jordan",
                PostalCode = "77110"
            },
            new()
            {
                Name = "Dubai",
                Country = "United Arab Emirates",
                PostalCode = "00000"
            },
            new()
            {
                Name = "Abu Dhabi",
                Country = "United Arab Emirates",
                PostalCode = "00000"
            },
            new()
            {
                Name = "Istanbul",
                Country = "Turkey",
                PostalCode = "34000"
            },
            new()
            {
                Name = "Cairo",
                Country = "Egypt",
                PostalCode = "11511"
            },
            new()
            {
                Name = "Beirut",
                Country = "Lebanon",
                PostalCode = "11000"
            },
            new()
            {
                Name = "Doha",
                Country = "Qatar",
                PostalCode = "00000"
            },
            new()
            {
                Name = "Riyadh",
                Country = "Saudi Arabia",
                PostalCode = "11564"
            },
            new()
            {
                Name = "Jeddah",
                Country = "Saudi Arabia",
                PostalCode = "21577"
            },
            new()
            {
                Name = "Muscat",
                Country = "Oman",
                PostalCode = "100"
            },
            new()
            {
                Name = "Manama",
                Country = "Bahrain",
                PostalCode = "00000"
            },
            new()
            {
                Name = "Kuwait City",
                Country = "Kuwait",
                PostalCode = "00000"
            },
            new()
            {
                Name = "Rabat",
                Country = "Morocco",
                PostalCode = "10000"
            },
            new()
            {
                Name = "Casablanca",
                Country = "Morocco",
                PostalCode = "20000"
            },
            new()
            {
                Name = "Marrakesh",
                Country = "Morocco",
                PostalCode = "40000"
            },
            new()
            {
                Name = "Athens",
                Country = "Greece",
                PostalCode = "10500"
            },
            new()
            {
                Name = "Rome",
                Country = "Italy",
                PostalCode = "00100"
            },
            new()
            {
                Name = "Paris",
                Country = "France",
                PostalCode = "75000"
            },
            new()
            {
                Name = "London",
                Country = "United Kingdom",
                PostalCode = "SW1A"
            }
        };

        await context.Cities.AddRangeAsync(
            cities,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 4. ROOM TYPES
        // ============================================================

        var roomTypes = new List<RoomType>
        {
            new()
            {
                Name = "Single",
                Description = "Comfortable room designed for one guest."
            },
            new()
            {
                Name = "Double",
                Description = "Spacious room with a double bed."
            },
            new()
            {
                Name = "Twin",
                Description = "Room with two separate beds."
            },
            new()
            {
                Name = "Triple",
                Description = "Room designed for three guests."
            },
            new()
            {
                Name = "Deluxe",
                Description = "Upgraded room with additional space and premium furnishings."
            },
            new()
            {
                Name = "Executive",
                Description = "Premium room designed for business and executive travelers."
            },
            new()
            {
                Name = "Suite",
                Description = "Large suite with separate living and sleeping areas."
            },
            new()
            {
                Name = "Junior Suite",
                Description = "Comfortable suite with an expanded living area."
            },
            new()
            {
                Name = "Family",
                Description = "Large room designed for families and groups."
            },
            new()
            {
                Name = "Presidential Suite",
                Description = "Luxury suite with premium facilities and spacious living areas."
            }
        };

        await context.RoomTypes.AddRangeAsync(
            roomTypes,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 5. AMENITIES
        // ============================================================

        var amenityData = new[]
        {
            ("Free WiFi", "High-speed wireless internet access."),
            ("Swimming Pool", "Outdoor or indoor swimming pool."),
            ("Parking", "On-site parking for hotel guests."),
            ("Gym", "Fully equipped fitness center."),
            ("Spa", "Spa and wellness facilities."),
            ("Breakfast", "Breakfast available for hotel guests."),
            ("Restaurant", "On-site restaurant."),
            ("Room Service", "Food and beverage room service."),
            ("Airport Shuttle", "Transportation service to and from the airport."),
            ("Air Conditioning", "Air conditioning available in rooms."),
            ("24 Hour Reception", "Front desk available around the clock."),
            ("Laundry", "Laundry and cleaning services."),
            ("Business Center", "Business facilities and work areas."),
            ("Bar", "On-site bar."),
            ("Beach Access", "Direct or nearby access to the beach."),
            ("Sea View", "Rooms with sea or ocean views."),
            ("City View", "Rooms with city views."),
            ("Pet Friendly", "Pets are allowed according to hotel policy."),
            ("Non Smoking", "Non-smoking rooms and areas."),
            ("Family Rooms", "Rooms suitable for families."),
            ("Conference Room", "Conference and meeting facilities."),
            ("Kids Club", "Activities and facilities for children."),
            ("Garden", "Hotel garden and outdoor spaces."),
            ("Terrace", "Outdoor terrace."),
            ("Balcony", "Rooms with private balconies."),
            ("Sauna", "Sauna facilities."),
            ("Jacuzzi", "Jacuzzi or hot tub."),
            ("Concierge", "Concierge services."),
            ("Valet Parking", "Valet parking service."),
            ("24 Hour Security", "Security service available around the clock.")
        };

        var amenities = amenityData
            .Select(x => new Amenity
            {
                Name = x.Item1,
                Description = x.Item2
            })
            .ToList();

        await context.Amenities.AddRangeAsync(
            amenities,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 6. HOTELS
        // ============================================================

        var hotelPrefixes = new[]
        {
            "Grand",
            "Royal",
            "Golden",
            "Elite",
            "Pearl",
            "Majestic",
            "Imperial",
            "Luxury",
            "Central",
            "Plaza",
            "Metropolitan",
            "Continental",
            "Royal Palace",
            "Grand Palace",
            "Heritage",
            "Regency",
            "Crown",
            "Garden",
            "Oasis",
            "Skyline"
        };

        var hotelSuffixes = new[]
        {
            "Hotel",
            "Resort",
            "Suites",
            "Residence",
            "Palace",
            "Inn",
            "Grand Hotel"
        };

        var hotels = new List<Hotel>();

        for (var i = 0; i < 60; i++)
        {
            var city = cities[i % cities.Count];
            var owner = owners[i % owners.Count];

            var prefix = hotelPrefixes[i % hotelPrefixes.Length];
            var suffix = hotelSuffixes[i % hotelSuffixes.Length];

            var name = $"{prefix} {city.Name} {suffix}";

            var starRating = 3m + (i % 3);

            hotels.Add(new Hotel
            {
                CityId = city.Id,
                OwnerId = owner.Id,
                Name = name,
                Description =
                    $"A {starRating}-star property located in {city.Name}. " +
                    $"The {name} offers comfortable accommodation, modern facilities, " +
                    "professional hospitality and convenient access to major attractions.",
                StarRating = starRating,
                Location = $"{city.Name} City Center"
            });
        }

        await context.Hotels.AddRangeAsync(
            hotels,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 7. HOTEL AMENITIES
        // ============================================================

        var hotelAmenities = new List<HotelAmenity>();

        foreach (var hotel in hotels)
        {
            var numberOfAmenities = 10 + (hotel.Id % 10);

            for (var i = 0; i < numberOfAmenities; i++)
            {
                var amenityIndex =
                    (hotel.Id * 3 + i * 7) % amenities.Count;

                var amenity = amenities[amenityIndex];

                var exists = hotelAmenities.Any(x =>
                    x.HotelId == hotel.Id &&
                    x.AmenityId == amenity.Id);

                if (!exists)
                {
                    hotelAmenities.Add(new HotelAmenity
                    {
                        HotelId = hotel.Id,
                        AmenityId = amenity.Id
                    });
                }
            }
        }

        await context.HotelAmenities.AddRangeAsync(
            hotelAmenities,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 8. HOTEL IMAGES
        // ============================================================

        var hotelImages = new List<HotelImage>();

        foreach (var hotel in hotels)
        {
            for (var imageNumber = 1; imageNumber <= 5; imageNumber++)
            {
                hotelImages.Add(new HotelImage
                {
                    HotelId = hotel.Id,
                    ImageUrl =
                        $"https://picsum.photos/seed/hotel-{hotel.Id}-{imageNumber}/1200/800",
                    PublicId =
                        $"seed-hotel-{hotel.Id}-{imageNumber}"
                });
            }
        }

        await context.HotelImages.AddRangeAsync(
            hotelImages,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 9. ROOMS
        // ============================================================

        var rooms = new List<Room>();

        foreach (var hotel in hotels)
        {
            // 20 rooms per hotel = 1,200 rooms total.
            for (var roomNumber = 1; roomNumber <= 20; roomNumber++)
            {
                var roomType =
                    roomTypes[
                        (hotel.Id + roomNumber) %
                        roomTypes.Count];

                var basePrice = hotel.StarRating switch
                {
                    >= 5 => 180m,
                    >= 4 => 120m,
                    _ => 70m
                };

                var roomTypeMultiplier = roomType.Name switch
                {
                    "Single" => 1.0m,
                    "Double" => 1.1m,
                    "Twin" => 1.1m,
                    "Triple" => 1.25m,
                    "Deluxe" => 1.5m,
                    "Executive" => 1.7m,
                    "Suite" => 2.0m,
                    "Junior Suite" => 1.8m,
                    "Family" => 1.6m,
                    "Presidential Suite" => 3.5m,
                    _ => 1.0m
                };

                var price =
                    (basePrice * roomTypeMultiplier) +
                    (roomNumber * 2.5m);

                var adultsCapacity = roomType.Name switch
                {
                    "Single" => 1,
                    "Double" => 2,
                    "Twin" => 2,
                    "Triple" => 3,
                    "Family" => 4,
                    "Suite" => 4,
                    "Junior Suite" => 3,
                    "Executive" => 2,
                    "Deluxe" => 2,
                    "Presidential Suite" => 6,
                    _ => 2
                };

                var childrenCapacity = roomType.Name switch
                {
                    "Family" => 3,
                    "Suite" => 2,
                    "Junior Suite" => 2,
                    "Presidential Suite" => 4,
                    _ => 1
                };

                rooms.Add(new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = $"{roomNumber:000}",
                    RoomTypeId = roomType.Id,
                    PricePerNight = decimal.Round(price, 2),
                    AdultsCapacity = adultsCapacity,
                    ChildrenCapacity = childrenCapacity,
                    Availability = true
                });
            }
        }

        await context.Rooms.AddRangeAsync(
            rooms,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 10. ROOM IMAGES
        // ============================================================

        var roomImages = new List<RoomImage>();

        foreach (var room in rooms)
        {
            for (var imageNumber = 1; imageNumber <= 3; imageNumber++)
            {
                roomImages.Add(new RoomImage
                {
                    RoomId = room.Id,
                    ImageUrl =
                        $"https://picsum.photos/seed/room-{room.Id}-{imageNumber}/1200/800",
                    PublicId =
                        $"seed-room-{room.Id}-{imageNumber}"
                });
            }
        }

        await context.RoomImages.AddRangeAsync(
            roomImages,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 11. DEALS
        // ============================================================

        var today = DateTime.UtcNow.Date;

        var deals = new List<Deal>();

        foreach (var hotel in hotels)
        {
            // Active deal.
            deals.Add(new Deal
            {
                HotelId = hotel.Id,
                DiscountPercentage =
                    10m + ((hotel.Id % 5) * 5m),
                StartDate = today.AddDays(-30),
                EndDate = today.AddDays(30)
            });

            // Future deal for every second hotel.
            if (hotel.Id % 2 == 0)
            {
                deals.Add(new Deal
                {
                    HotelId = hotel.Id,
                    DiscountPercentage =
                        15m + ((hotel.Id % 3) * 5m),
                    StartDate = today.AddDays(45),
                    EndDate = today.AddDays(90)
                });
            }

            // Another future deal for every fifth hotel.
            if (hotel.Id % 5 == 0)
            {
                deals.Add(new Deal
                {
                    HotelId = hotel.Id,
                    DiscountPercentage = 20m,
                    StartDate = today.AddDays(100),
                    EndDate = today.AddDays(140)
                });
            }
        }

        await context.Deals.AddRangeAsync(
            deals,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 12. REVIEWS
        // ============================================================

        var positiveReviewTexts = new[]
        {
            "Excellent stay and very helpful staff.",
            "The room was clean and comfortable.",
            "Great location and excellent facilities.",
            "Very comfortable hotel. Would definitely stay again.",
            "The staff were friendly and professional.",
            "Good value for the price.",
            "The room was spacious and clean.",
            "Breakfast was excellent.",
            "The location was perfect for our trip.",
            "Everything was exactly as expected.",
            "Beautiful hotel with excellent service.",
            "The facilities were very clean.",
            "The check-in process was quick and easy.",
            "Great experience from start to finish.",
            "Very good hotel for a family trip."
        };

        var neutralReviewTexts = new[]
        {
            "Overall a good experience.",
            "The room was comfortable.",
            "Good location and reasonable facilities.",
            "The hotel was fine for a short stay.",
            "Everything was acceptable.",
            "The staff were helpful.",
            "Good hotel with a few areas for improvement."
        };

        var reviews = new List<Review>();

        for (var i = 0; i < 1200; i++)
        {
            var customer = customers[i % customers.Count];
            var hotel = hotels[(i * 7) % hotels.Count];

            var rating = i % 10 switch
            {
                0 => 3,
                1 => 3,
                2 => 4,
                3 => 4,
                4 => 4,
                5 => 5,
                6 => 5,
                7 => 5,
                8 => 5,
                _ => 4
            };

            var comment = rating >= 4
                ? positiveReviewTexts[i % positiveReviewTexts.Length]
                : neutralReviewTexts[i % neutralReviewTexts.Length];

            reviews.Add(new Review
            {
                UserId = customer.Id,
                HotelId = hotel.Id,
                Rating = rating,
                Comment = comment
            });
        }

        await context.Reviews.AddRangeAsync(
            reviews,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 13. BOOKINGS
        // ============================================================
        //
        // We deliberately generate at most one booking per room.
        // This prevents accidental overlapping bookings for the same
        // room while still providing a large booking dataset.
        //
        // 1,000 bookings out of 1,200 rooms.
        // ============================================================

        var bookings = new List<Booking>();

        for (var i = 0; i < 1000; i++)
        {
            var customer = customers[
                (i * 13) % customers.Count];

            var room = rooms[i];

            // Spread bookings across past and future dates.
            var checkIn =
                today.AddDays(-180 + ((i * 17) % 330));

            var nights =
                1 + ((i * 7) % 7);

            var checkOut =
                checkIn.AddDays(nights);

            var totalPrice =
                decimal.Round(
                    room.PricePerNight * nights,
                    2);

            bookings.Add(new Booking
            {
                UserId = customer.Id,
                RoomId = room.Id,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalPrice = totalPrice,
                Status = GetBookingStatus(i)
            });
        }

        await context.Bookings.AddRangeAsync(
            bookings,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        // ============================================================
        // 14. PAYMENTS
        // ============================================================

        var payments = new List<Payment>();

        foreach (var booking in bookings)
        {
            PaymentStatus? paymentStatus = booking.Status switch
            {
                BookingStatus.Pending =>
                    null,

                BookingStatus.Confirmed =>
                    PaymentStatus.Success,

                BookingStatus.Completed =>
                    PaymentStatus.Success,

                BookingStatus.Cancelled =>
                    PaymentStatus.Refunded,

                _ =>
                    null
            };

            if (paymentStatus is null)
            {
                continue;
            }

            var paymentDate =
                new DateTimeOffset(
                    booking.CheckInDate,
                    TimeSpan.Zero);

            payments.Add(new Payment
            {
                BookingId = booking.Id,
                Provider = "Stripe",
                TransactionId = $"seed-tx-{booking.Id}",
                PaymentIntentId = $"seed-pi-{booking.Id}",
                IdempotencyKey = $"seed-idempotency-{booking.Id}",
                CheckoutUrl =
                    $"https://checkout.stripe.com/seed/{booking.Id}",
                Amount = booking.TotalPrice,
                Status = paymentStatus.Value,
                PaymentDate = paymentDate
            });
        }

        await context.Payments.AddRangeAsync(
            payments,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    // ================================================================
    // USER CREATION
    // ================================================================

    private static User CreateUser(
        string email,
        string password,
        UserRole role,
        IPasswordHasher<User> passwordHasher)
    {
        var user = new User
        {
            Email = email.Trim().ToLowerInvariant(),
            Role = role
        };

        user.PasswordHash =
            passwordHasher.HashPassword(
                user,
                password);

        return user;
    }

    // ================================================================
    // BOOKING STATUS
    // ================================================================

    private static BookingStatus GetBookingStatus(int index)
    {
        return (index % 10) switch
        {
            0 => BookingStatus.Pending,
            1 => BookingStatus.Cancelled,
            2 => BookingStatus.Confirmed,
            3 => BookingStatus.Confirmed,
            4 => BookingStatus.Completed,
            5 => BookingStatus.Completed,
            6 => BookingStatus.Completed,
            7 => BookingStatus.Confirmed,
            8 => BookingStatus.Completed,
            _ => BookingStatus.Confirmed
        };
    }
}