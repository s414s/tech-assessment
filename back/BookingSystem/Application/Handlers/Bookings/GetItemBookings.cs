using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Handlers.Bookings;

public sealed record GetItemBookingsQuery(DateTimeOffset From, DateTimeOffset To, long ItemId);
public sealed record GetItemBookingsResponse(IEnumerable<BookingDto> Bookings);
public sealed record BookingDto(DateTimeOffset From, DateTimeOffset To, long UserId);

public class GetItemBookingsHandler : IHandler<GetItemBookingsQuery, GetItemBookingsResponse>
{
    private readonly IRepository<Booking> _repository;

    public GetItemBookingsHandler(IRepository<Booking> repository)
    {
        _repository = repository;
    }

    public async Task<GetItemBookingsResponse> HandleAsync(GetItemBookingsQuery request)
    {
        // Npgsql only accepts UTC (offset 0) values when comparing against timestamptz columns
        var from = request.From.ToUniversalTime();
        var to = request.To.ToUniversalTime();

        if (to <= from)
            throw new DomainException("End of the range must be after its start.");

        // Any booking overlapping the range, including ones that start before or end after it
        var bookings = await _repository.Query
            .Where(x => x.ItemId == request.ItemId && x.From < to && x.To > from)
            .OrderBy(x => x.From)
            .Select(x => new BookingDto(x.From, x.To, x.UserId))
            .ToListAsync();

        return new GetItemBookingsResponse(bookings);
    }
}
