using BookingSystem.Domain.Enums;
using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Domain.Entities;

public class Item : Entity
{
    public required string Name { get; set; }
    public ItemType Type { get; set; }

    public long LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; } = [];

    // The item's slots on that (Madrid) day, from the location's opening to its closing time, each with the user
    // who booked it (if any). Weekends have no slots. Needs Location and that day's Bookings loaded.
    public IReadOnlyList<Slot> GetSlotsOn(DateOnly day)
    {
        if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return [];

        var closing = Location.FromLocal(day, Location.ClosingTime);
        var slots = new List<Slot>();

        for (var from = Location.FromLocal(day, Location.OpeningTime); from + Booking.SlotLength <= closing; from += Booking.SlotLength)
        {
            var to = from + Booking.SlotLength;
            var booking = Bookings.FirstOrDefault(b => b.From < to && b.To > from);

            // Returned in UTC, like the stored bookings
            slots.Add(new Slot(from.ToUniversalTime(), to.ToUniversalTime(), booking?.UserId));
        }

        return slots;
    }

    // Whether at least one slot on that day can still be booked, following the same rules as Booking.Create:
    // weekdays only, within the location's opening hours and from the next slot onwards
    public bool HasFreeSlotOn(DateOnly day, DateTimeOffset now)
    {
        var nextSlot = Booking.RoundUpToNextSlotBoundary(now.ToUniversalTime());

        return GetSlotsOn(day).Any(x => x.IsFree && x.From >= nextSlot);
    }
}
