namespace BookingSystem.Application.Handlers.Bookings;

public sealed record CreateBookingCommand(DateTime From, DateTime To, long ItemId);
public sealed record CreateBookingResponse(long Id);

public sealed record CreateBookingHandler
{

}

