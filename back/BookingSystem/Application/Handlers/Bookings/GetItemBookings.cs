using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Handlers.Bookings;

public sealed record GetItemBookingsQuery(DateTime From, DateTime To, long ItemId);
public sealed record GetItemBookingsResponse(IEnumerable<BookingDto> Bookings);
public sealed record BookingDto(DateTime From, DateTime To, long userId);

public class GetItemBookingsHandler
{
    public async Task<GetItemBookingsResponse> HandleAsync(GetItemBookingsQuery query, IRepository<Booking> repository)
    {
        var bookings = await repository.Query.Where(x => x.Id == query.ItemId).FirstOrDefaultAsync();

        throw new NotImplementedException();
    }
}
