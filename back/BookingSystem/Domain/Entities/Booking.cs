namespace BookingSystem.Domain.Entities;

public class Booking : Entity
{
    private static readonly TimeSpan _stepGap = TimeSpan.FromMinutes(15);

    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;

    public static Booking Create(DateTimeOffset from, DateTimeOffset to, User madeBy, Item item)
    {
        if (to <= from)
            throw new ArgumentException("End time must be after start time.");

        if (!IsValidTime(from) || !IsValidTime(to))
            throw new ArgumentException($"Booking times must align with {_stepGap.TotalMinutes}-minute intervals.");

        return new Booking()
        {
            From = from,
            To = to,
            User = madeBy,
            Item = item,
        };
    }

    private static bool IsValidTime(DateTimeOffset time) => time.TimeOfDay.Ticks % _stepGap.Ticks == 0;
}


