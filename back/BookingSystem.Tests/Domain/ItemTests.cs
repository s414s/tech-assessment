using BookingSystem.Domain.Entities;

namespace BookingSystem.Tests.Domain;

public class ItemTests
{
    // Monday; the location is open 09:00-18:00 Madrid time (UTC+1 that day)
    private static readonly DateOnly _monday = new(2026, 10, 26);

    private static DateTimeOffset Madrid(int hour, int minute, int day = 26) =>
        Location.FromLocal(new DateOnly(2026, 10, day), new TimeOnly(hour, minute));

    // The previous Friday, so "now" never limits the day unless a test says so
    private static readonly DateTimeOffset _now = Madrid(12, 0, day: 23);

    private static Item ItemWith(params (DateTimeOffset From, DateTimeOffset To)[] bookings) => new()
    {
        Name = "Test item",
        Location = new Location { Name = "Test location" },
        Bookings = bookings.Select(b => new Booking { From = b.From, To = b.To }).ToList(),
    };

    [Fact]
    public void GetSlotsOn_CoversOpeningHoursInQuarterSteps()
    {
        var slots = ItemWith().GetSlotsOn(_monday);

        Assert.Equal(36, slots.Count); // 09:00-18:00
        Assert.Equal(Madrid(9, 0), slots[0].From);
        Assert.Equal(Madrid(9, 15), slots[0].To);
        Assert.Equal(Madrid(17, 45), slots[^1].From);
        Assert.Equal(Madrid(18, 0), slots[^1].To);
        Assert.All(slots, x => Assert.Equal(TimeSpan.Zero, x.From.Offset));
        Assert.All(slots, x => Assert.True(x.IsFree));
    }

    [Fact]
    public void GetSlotsOn_MarksBookedSlotsWithTheirUser()
    {
        var item = ItemWith();
        item.Bookings.Add(new Booking { From = Madrid(10, 0), To = Madrid(10, 30), UserId = 7 });

        var slots = item.GetSlotsOn(_monday);

        var booked = slots.Where(x => !x.IsFree).ToList();
        Assert.Equal(2, booked.Count);
        Assert.Equal(Madrid(10, 0), booked[0].From);
        Assert.Equal(Madrid(10, 15), booked[1].From);
        Assert.All(booked, x => Assert.Equal(7, x.UserId));
    }

    [Theory]
    [InlineData(2026, 10, 24)] // Saturday
    [InlineData(2026, 10, 25)] // Sunday
    public void GetSlotsOn_Weekend_ReturnsEmpty(int year, int month, int day)
    {
        var date = new DateOnly(year, month, day);

        Assert.Empty(ItemWith().GetSlotsOn(date));
    }

    [Fact]
    public void HasFreeSlotOn_WithoutBookings_IsTrue()
    {
        Assert.True(ItemWith().HasFreeSlotOn(_monday, _now));
    }

    [Fact]
    public void HasFreeSlotOn_FullyBooked_IsFalse()
    {
        var item = ItemWith((Madrid(9, 0), Madrid(18, 0)));

        Assert.False(item.HasFreeSlotOn(_monday, _now));
    }

    [Fact]
    public void HasFreeSlotOn_FullyBookedAsStoredInUtc_IsFalse()
    {
        // 08:00-17:00 UTC is 09:00-18:00 in Madrid on that day
        var from = new DateTimeOffset(2026, 10, 26, 8, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 10, 26, 17, 0, 0, TimeSpan.Zero);

        Assert.False(ItemWith((from, to)).HasFreeSlotOn(_monday, _now));
    }

    [Fact]
    public void HasFreeSlotOn_FullyBookedWithBackToBackBookings_IsFalse()
    {
        var item = ItemWith((Madrid(13, 0), Madrid(18, 0)), (Madrid(9, 0), Madrid(13, 0)));

        Assert.False(item.HasFreeSlotOn(_monday, _now));
    }

    [Theory]
    [InlineData(9, 15, 18, 0)]  // free 09:00-09:15
    [InlineData(9, 0, 17, 45)]  // free 17:45-18:00
    public void HasFreeSlotOn_WithOneSlotFreeAtAnEnd_IsTrue(int fromHour, int fromMinute, int toHour, int toMinute)
    {
        var item = ItemWith((Madrid(fromHour, fromMinute), Madrid(toHour, toMinute)));

        Assert.True(item.HasFreeSlotOn(_monday, _now));
    }

    [Fact]
    public void HasFreeSlotOn_WithOneSlotFreeBetweenBookings_IsTrue()
    {
        var item = ItemWith((Madrid(9, 0), Madrid(12, 0)), (Madrid(12, 15), Madrid(18, 0)));

        Assert.True(item.HasFreeSlotOn(_monday, _now));
    }

    [Fact]
    public void HasFreeSlotOn_IgnoresBookingsOutsideOpeningHours()
    {
        var item = ItemWith((Madrid(7, 0), Madrid(9, 0)), (Madrid(9, 0), Madrid(17, 45)), (Madrid(18, 0), Madrid(20, 0)));

        Assert.True(item.HasFreeSlotOn(_monday, _now));
    }

    [Theory]
    [InlineData(24)] // Saturday
    [InlineData(25)] // Sunday
    public void HasFreeSlotOn_Weekend_IsFalse(int day)
    {
        Assert.False(ItemWith().HasFreeSlotOn(new DateOnly(2026, 10, day), _now));
    }

    [Fact]
    public void HasFreeSlotOn_TodayWithOnlyPassedSlotsFree_IsFalse()
    {
        // Free 09:00-12:00, but it is already 12:00
        var item = ItemWith((Madrid(12, 0), Madrid(18, 0)));

        Assert.False(item.HasFreeSlotOn(_monday, Madrid(12, 0)));
    }

    [Fact]
    public void HasFreeSlotOn_TodayWithLastSlotStillAhead_IsTrue()
    {
        // At 17:40 the next slot is 17:45, which still fits before closing
        Assert.True(ItemWith().HasFreeSlotOn(_monday, Madrid(17, 40)));
    }

    [Fact]
    public void HasFreeSlotOn_TodayAfterLastSlotStarted_IsFalse()
    {
        // At 17:50 the next slot would start at 18:00, when the location closes
        Assert.False(ItemWith().HasFreeSlotOn(_monday, Madrid(17, 50)));
    }

    [Fact]
    public void HasFreeSlotOn_PastDay_IsFalse()
    {
        Assert.False(ItemWith().HasFreeSlotOn(_monday, Madrid(9, 0, day: 27)));
    }
}
