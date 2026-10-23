using BookingSystem.Domain.Enums;

namespace BookingSystem.Domain.Entities;

public class Item : Entity
{
    public required string Name { get; set; }
    public ItemType Type { get; set; }

    public long LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; } = [];

    // Whether at least one slot on that (Madrid) day can still be booked, following the same rules as Booking.Create:
    // weekdays only, within the location's opening hours and from the next slot onwards. Needs Location and Bookings loaded.
    public bool HasFreeSlotOn(DateOnly day, DateTimeOffset now)
    {
        if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        var opening = Location.At(day, Location.OpeningTime);
        var closing = Location.At(day, Location.ClosingTime);

        var nextSlot = Booking.RoundUp(now.ToUniversalTime());
        var cursor = nextSlot > opening ? nextSlot : opening;

        // Walk the bookings in order looking for a gap of at least one slot
        foreach (var booking in Bookings.Where(b => b.From < closing && b.To > cursor).OrderBy(b => b.From))
        {
            if (booking.From - cursor >= Booking.SlotLength)
                return true;

            if (booking.To > cursor)
                cursor = booking.To;
        }

        return closing - cursor >= Booking.SlotLength;
    }
}
