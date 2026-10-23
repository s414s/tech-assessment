using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Tests.Domain;

public class BookingTests
{
    private static readonly User _user = new() { Id = 1, Name = "Test user" };
    private static readonly Location _location = new() { Id = 1, Name = "Test location" }; // Open 09:00-18:00 Madrid time
    private static readonly Item _item = new() { Id = 1, Name = "Test item", Type = default(ItemType), LocationId = 1, Location = _location };

    // Madrid wall-clock time in October 2026: UTC+2 until Sunday the 25th, UTC+1 from then on
    private static DateTimeOffset Madrid(int hour, int minute, int day = 23) =>
        Location.At(new DateOnly(2026, 10, day), new TimeOnly(hour, minute));

    private static DateTimeOffset Utc(int hour, int minute, int day) =>
        new(2026, 10, day, hour, minute, 0, TimeSpan.Zero);

    // Before opening on the booking day (a Friday), so only the opening-hours rules apply by default
    private static readonly DateTimeOffset _now = Madrid(7, 0);

    private static Booking Create(DateTimeOffset from, DateTimeOffset to, DateTimeOffset? now = null) =>
        Booking.Create(from, to, _user, _item, now ?? _now);

    [Fact]
    public void Create_WithValidRange_AssignsUserItemAndTimes()
    {
        var booking = Create(Madrid(10, 0), Madrid(11, 30));

        Assert.Same(_user, booking.User);
        Assert.Same(_item, booking.Item);
        Assert.Equal(Madrid(10, 0), booking.From);
        Assert.Equal(Madrid(11, 30), booking.To);
    }

    [Theory]
    [InlineData(10, 1, 10, 15)]
    [InlineData(10, 14, 10, 15)]
    [InlineData(10, 16, 10, 30)]
    [InlineData(10, 50, 11, 0)]
    public void Create_WithTimeOffQuarter_RoundsUpToNextQuarter(int hour, int minute, int expectedHour, int expectedMinute)
    {
        var booking = Create(Madrid(hour, minute), Madrid(14, 0));

        Assert.Equal(Madrid(expectedHour, expectedMinute), booking.From);
    }

    [Fact]
    public void Create_WithTimeOneTickPastQuarter_RoundsUpToNextQuarter()
    {
        var booking = Create(Madrid(10, 0).AddTicks(1), Madrid(11, 0));

        Assert.Equal(Madrid(10, 15), booking.From);
    }

    [Fact]
    public void Create_WithTimeOnQuarter_DoesNotRound()
    {
        var booking = Create(Madrid(10, 45), Madrid(11, 15));

        Assert.Equal(Madrid(10, 45), booking.From);
        Assert.Equal(Madrid(11, 15), booking.To);
    }

    [Fact]
    public void Create_StoresTimesInUtc()
    {
        var booking = Create(Madrid(10, 0), Madrid(11, 0));

        Assert.Equal(TimeSpan.Zero, booking.From.Offset);
        Assert.Equal(TimeSpan.Zero, booking.To.Offset);
        Assert.Equal(Utc(8, 0, day: 23), booking.From);
        Assert.Equal(Utc(9, 0, day: 23), booking.To);
    }

    [Fact]
    public void Create_WithEndBeforeStartInDifferentOffsets_Throws()
    {
        // 09:30 UTC is 11:30 in Madrid, so it ends before an 11:45 Madrid start
        Assert.Throws<DomainException>(() => Create(Madrid(11, 45), Utc(9, 30, day: 23)));
    }

    [Fact]
    public void Create_WithEndEqualToStart_Throws()
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(10, 0), Madrid(10, 0)));

        Assert.Equal("End time must be after start time.", exception.Message);
    }

    [Fact]
    public void Create_WithEndBeforeStart_Throws()
    {
        Assert.Throws<DomainException>(() => Create(Madrid(11, 0), Madrid(10, 0)));
    }

    [Fact]
    public void Create_WithDistinctTimesRoundingToSameQuarter_Throws()
    {
        // Both round up to 10:15, leaving an empty booking
        Assert.Throws<DomainException>(() => Create(Madrid(10, 1), Madrid(10, 14)));
    }

    [Fact]
    public void Create_OnPastDate_Throws()
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(10, 0, day: 22), Madrid(11, 0, day: 22)));

        Assert.Equal("Bookings cannot be made for past dates.", exception.Message);
    }

    [Fact]
    public void Create_EarlierTodayThanNextSlot_Throws()
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(10, 0), Madrid(11, 0), now: Madrid(10, 7)));

        Assert.Equal("Bookings must start at 10:15 or later.", exception.Message);
    }

    [Fact]
    public void Create_AtNextSlot_Succeeds()
    {
        var booking = Create(Madrid(10, 15), Madrid(11, 0), now: Madrid(10, 7));

        Assert.Equal(Madrid(10, 15), booking.From);
    }

    [Fact]
    public void Create_InsideCurrentSlot_RoundsUpToNextSlot()
    {
        // 10:08 rounds up to 10:15, which is the next slot after 10:07
        var booking = Create(Madrid(10, 8), Madrid(11, 0), now: Madrid(10, 7));

        Assert.Equal(Madrid(10, 15), booking.From);
    }

    [Fact]
    public void Create_OnFutureDay_Succeeds()
    {
        var booking = Create(Madrid(9, 0, day: 26), Madrid(10, 0, day: 26), now: Madrid(17, 0));

        Assert.Equal(Madrid(9, 0, day: 26), booking.From);
    }

    [Theory]
    [InlineData(8, 45, 10, 0)]
    [InlineData(17, 0, 18, 15)]
    [InlineData(18, 0, 18, 30)]
    public void Create_OutsideOpeningHours_Throws(int fromHour, int fromMinute, int toHour, int toMinute)
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(fromHour, fromMinute), Madrid(toHour, toMinute)));

        Assert.Equal("Test location is only open from 09:00 to 18:00.", exception.Message);
    }

    [Fact]
    public void Create_CoveringFullOpeningHours_Succeeds()
    {
        var booking = Create(Madrid(9, 0), Madrid(18, 0));

        Assert.Equal(Madrid(9, 0), booking.From);
        Assert.Equal(Madrid(18, 0), booking.To);
    }

    [Theory]
    [InlineData(23, 7)] // Friday, summer time (UTC+2): 09:00 Madrid is 07:00 UTC
    [InlineData(26, 8)] // Monday, winter time (UTC+1): 09:00 Madrid is 08:00 UTC
    public void Create_WithUtcTimesAtMadridOpening_Succeeds(int day, int utcHour)
    {
        // What the frontend sends for 09:00-10:00 picked in Spain
        var booking = Create(Utc(utcHour, 0, day), Utc(utcHour + 1, 0, day));

        Assert.Equal(Madrid(9, 0, day), booking.From);
    }

    [Theory]
    [InlineData(23, 6)] // 08:00 Madrid in summer time
    [InlineData(26, 7)] // 08:00 Madrid in winter time
    public void Create_WithUtcTimesBeforeMadridOpening_Throws(int day, int utcHour)
    {
        var exception = Assert.Throws<DomainException>(() => Create(Utc(utcHour, 0, day), Utc(utcHour + 1, 0, day)));

        Assert.Equal("Test location is only open from 09:00 to 18:00.", exception.Message);
    }

    [Theory]
    [InlineData(24)] // Saturday
    [InlineData(25)] // Sunday
    public void Create_OnWeekend_Throws(int day)
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(10, 0, day), Madrid(11, 0, day)));

        Assert.Equal("Bookings cannot be made on weekends.", exception.Message);
    }

    [Fact]
    public void Create_OnWeekendOnlyInMadrid_Throws()
    {
        // Friday 22:15-22:30 UTC is already Saturday 00:15-00:30 in Madrid
        var exception = Assert.Throws<DomainException>(() => Create(Utc(22, 15, day: 23), Utc(22, 30, day: 23)));

        Assert.Equal("Bookings cannot be made on weekends.", exception.Message);
    }

    [Fact]
    public void Create_SpanningTwoDays_Throws()
    {
        var exception = Assert.Throws<DomainException>(() => Create(Madrid(10, 0), Madrid(10, 0, day: 24)));

        Assert.Equal("Booking must start and end on the same day.", exception.Message);
    }
}
