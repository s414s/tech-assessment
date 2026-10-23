using BookingSystem.Application.Common;
using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Handlers.Items;

public sealed record GetFilteredItemsQuery(ItemType? Type, long? LocationId, DateOnly? AvailableOnDay, int Page, int Limit);
public sealed record GetFilteredItemsResponse(PaginatedList<ItemDto> Items);
public sealed record ItemDto(
    long Id,
    string Name,
    ItemType Type,
    long LocationId,
    string LocationName,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime,
    IEnumerable<ItemBookingDto> Bookings);
public sealed record ItemBookingDto(DateTimeOffset From, DateTimeOffset To, long UserId);

public class GetFilteredItemsHandler : IHandler<GetFilteredItemsQuery, GetFilteredItemsResponse>
{
    private readonly IRepository<Item> _repository;
    private readonly TimeProvider _timeProvider;

    public GetFilteredItemsHandler(IRepository<Item> repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<GetFilteredItemsResponse> HandleAsync(GetFilteredItemsQuery request)
    {
        var query = ApplyFilters(_repository.Query, request)
            .Include(x => x.Location)
            .OrderBy(x => x.Name);

        if (!request.AvailableOnDay.HasValue)
        {
            var page = await PaginatedList<Item>.CreateAsync(query, request.Page, request.Limit);
            return new GetFilteredItemsResponse(page.Map(ToDto));
        }

        // Free slots depend on opening hours and the current time, so they are worked out in memory
        // from that day's bookings; the item list is small enough to paginate afterwards
        // The day is a Madrid calendar day; Npgsql needs its bounds in UTC
        var day = request.AvailableOnDay.Value;
        var dayStart = Location.At(day, TimeOnly.MinValue).ToUniversalTime();
        var dayEnd = Location.At(day.AddDays(1), TimeOnly.MinValue).ToUniversalTime();
        var now = _timeProvider.GetUtcNow();

        var items = await query
            .Include(x => x.Bookings.Where(b => b.From < dayEnd && b.To > dayStart).OrderBy(b => b.From))
            .ToListAsync();

        var available = items
            .Where(x => x.HasFreeSlotOn(day, now))
            .Select(ToDto);

        return new GetFilteredItemsResponse(PaginatedList<ItemDto>.Create(available, request.Page, request.Limit));
    }

    private static IQueryable<Item> ApplyFilters(IQueryable<Item> query, GetFilteredItemsQuery request)
    {
        if (request.Type.HasValue)
            query = query.Where(x => x.Type == request.Type.Value);

        if (request.LocationId.HasValue)
            query = query.Where(x => x.LocationId == request.LocationId.Value);

        return query;
    }

    // Bookings are only loaded when filtering by day, so they are empty otherwise
    private static ItemDto ToDto(Item item) => new(
        item.Id,
        item.Name,
        item.Type,
        item.LocationId,
        item.Location.Name,
        item.Location.OpeningTime,
        item.Location.ClosingTime,
        item.Bookings.Select(b => new ItemBookingDto(b.From, b.To, b.UserId)).ToList());
}
