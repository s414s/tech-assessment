using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Context;

// Bookings are seeded at runtime (not with HasData) so their dates are relative to today
internal static class BookingSeeder
{
    // Each user only books items of one location: User 1 -> Center (items 1-5), User 2 -> Sol (items 6-10).
    // (user id, day offset from today, item id, start hour in Madrid time, duration in hours), within opening hours 09:00-18:00
    private static readonly (long UserId, int Day, long ItemId, int Hour, int Hours)[] _seeds =
    [
        (1, 0, 1, 9, 1),
        (1, 0, 2, 11, 2),
        (1, 0, 5, 15, 1),
        (1, 1, 3, 9, 3),
        (1, 2, 4, 10, 1),
        (1, 3, 1, 9, 2),
        (1, 5, 2, 9, 8),

        (2, 1, 6, 12, 1),
        (2, 1, 9, 14, 2),
        (2, 2, 8, 13, 4),
        (2, 3, 10, 16, 1),
        (2, 4, 7, 11, 1),
        (2, 5, 6, 10, 2),
    ];

    public static void Seed(DbContext context)
    {
        var bookings = context.Set<Booking>();
        if (bookings.Any()) return;

        bookings.AddRange(BuildBookings());
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        var bookings = context.Set<Booking>();
        if (await bookings.AnyAsync(cancellationToken)) return;

        bookings.AddRange(BuildBookings());
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Booking> BuildBookings()
    {
        var today = DateOnly.FromDateTime(Location.ToLocal(DateTimeOffset.UtcNow).DateTime);

        // Days are relative to today, so whichever seeds land on a weekend are skipped (weekends are not bookable)
        return _seeds
            .Select(seed => (seed, Day: today.AddDays(seed.Day)))
            .Where(x => x.Day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
            .Select(x =>
            {
                var from = Location.At(x.Day, new TimeOnly(x.seed.Hour, 0));
                return new Booking
                {
                    // Npgsql only stores UTC (offset 0) values in timestamptz columns
                    From = from.ToUniversalTime(),
                    To = from.AddHours(x.seed.Hours).ToUniversalTime(),
                    UserId = x.seed.UserId,
                    ItemId = x.seed.ItemId,
                };
            });
    }
}
