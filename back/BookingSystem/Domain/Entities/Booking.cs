using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Booking : Entity
{
    public static readonly TimeSpan SlotLength = TimeSpan.FromMinutes(15);

    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public long UserId { get; set; }
    public long ItemId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;

    // Days and times of day are checked in the location's time zone, whatever offset the caller used
    public static Booking Create(DateTimeOffset from, DateTimeOffset to, User madeBy, Item item, DateTimeOffset now)
    {
        from = Location.ToLocal(RoundUp(from));
        to = Location.ToLocal(RoundUp(to));

        if (to <= from)
            throw new DomainException("End time must be after start time.");

        var localNow = Location.ToLocal(now);
        if (from.Date < localNow.Date)
            throw new DomainException("Bookings cannot be made for past dates.");

        var nextSlot = RoundUp(localNow);
        if (from < nextSlot)
            throw new DomainException($"Bookings must start at {nextSlot:HH:mm} or later.");

        if (from.Date != to.Date)
            throw new DomainException("Booking must start and end on the same day.");

        if (from.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new DomainException("Bookings cannot be made on weekends.");

        var location = item.Location;
        if (TimeOnly.FromTimeSpan(from.TimeOfDay) < location.OpeningTime
            || TimeOnly.FromTimeSpan(to.TimeOfDay) > location.ClosingTime)
            throw new DomainException(
                $"{location.Name} is only open from {location.OpeningTime:HH:mm} to {location.ClosingTime:HH:mm}.");

        return new Booking
        {
            // Npgsql only stores UTC (offset 0) values in timestamptz columns
            From = from.ToUniversalTime(),
            To = to.ToUniversalTime(),
            User = madeBy,
            Item = item,
        };
    }

    // Rounds up to the next slot boundary (unchanged when already on one)
    public static DateTimeOffset RoundUp(DateTimeOffset time)
    {
        long ticks = SlotLength.Ticks;
        long remainder = time.Ticks % ticks;

        return remainder == 0
            ? time
            : time.AddTicks(ticks - remainder);
    }
}
