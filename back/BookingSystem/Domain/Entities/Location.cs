namespace BookingSystem.Domain.Entities;

public class Location : Entity
{
    // All offices are in Spain: opening hours, weekdays and calendar days are in Madrid time (DST included)
    public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");
    public static readonly TimeOnly OpeningTime = new(9, 0, 0);
    public static readonly TimeOnly ClosingTime = new(18, 0, 0);

    public required string Name { get; set; }

    public virtual ICollection<Item> Items { get; set; } = [];

    // The same instant, expressed in Madrid time
    public static DateTimeOffset ToLocal(DateTimeOffset time) => TimeZoneInfo.ConvertTime(time, TimeZone);

    // The exact moment it is `time` on the clock in Madrid on `day`, carrying Madrid's UTC offset for that date:
    // +01:00 in winter, +02:00 in summer. E.g. (2026-10-26, 09:00) -> 2026-10-26T09:00+01:00 (08:00 UTC)
    public static DateTimeOffset FromLocal(DateOnly day, TimeOnly time)
    {
        var local = day.ToDateTime(time);
        return new DateTimeOffset(local, TimeZone.GetUtcOffset(local));
    }
}
