using BookingSystem.Application.Common;
using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Handlers.Bookings;

public sealed record GetFilteredBookingsQuery(long UserId, int Page, int Limit);
public sealed record GetFilteredBookingsResponse(PaginatedList<BookingInfoDto> Bookings);
public sealed record BookingInfoDto(long Id, DateTimeOffset From, DateTimeOffset To, long LocationId, string LocationName, string ItemName);

public class GetFilteredBookingsHandler : IHandler<GetFilteredBookingsQuery, GetFilteredBookingsResponse>
{
    private readonly IRepository<Booking> _repository;

    public GetFilteredBookingsHandler(IRepository<Booking> repository)
    {
        _repository = repository;
    }

    public async Task<GetFilteredBookingsResponse> HandleAsync(GetFilteredBookingsQuery request)
    {
        var query = _repository.Query
            .Where(x => x.UserId == request.UserId)
            .OrderBy(x => x.From)
            .Select(x => new BookingInfoDto(x.Id, x.From, x.To, x.Item.LocationId, x.Item.Location.Name, x.Item.Name));

        return new GetFilteredBookingsResponse(await PaginatedList<BookingInfoDto>.CreateAsync(query, request.Page, request.Limit));
    }
}
