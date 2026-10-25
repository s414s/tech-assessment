namespace BookingSystem.Domain.ValueObjects;

public sealed record Slot(DateTimeOffset From, DateTimeOffset To, long? UserId)
{
    public bool IsFree => UserId is null;
}
