namespace BookingSystem.Domain.Entities;

public class Location : Entity
{
    // All offices are in Spain: opening hours, weekdays and calendar days are in Madrid time (DST included)
    public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");

    public required string Name { get; set; }
    public TimeOnly OpeningTime { get => new(9, 0, 0); }
    public TimeOnly ClosingTime { get => new(18, 0, 0); }

    public virtual ICollection<Item> Items { get; set; } = [];

    // The same instant, expressed in Madrid time
    public static DateTimeOffset ToLocal(DateTimeOffset time) => TimeZoneInfo.ConvertTime(time, TimeZone);

    // That Madrid wall-clock time on that day, with the offset in force then
    public static DateTimeOffset At(DateOnly day, TimeOnly time)
    {
        var local = day.ToDateTime(time);
        return new DateTimeOffset(local, TimeZone.GetUtcOffset(local));
    }
}
