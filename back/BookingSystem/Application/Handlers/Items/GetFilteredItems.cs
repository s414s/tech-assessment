using BookingSystem.Application.Common;
using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Handlers.Items;

public sealed record GetFilteredItemsQuery(
    ItemType? Type,
    long? LocationId,
    DateOnly? AvailableOnDay,
    int Page,
    int Limit);

public sealed record GetFilteredItemsResponse(PaginatedList<ItemDto> Items);

public sealed record ItemDto(
    long Id,
    string Name,
    ItemType Type,
    long LocationId,
    string LocationName,
    IEnumerable<ItemBookingDto> Bookings);

// One slot of the day; UserId is null when the slot is free and set when someone booked it
public sealed record ItemBookingDto(
    DateTimeOffset From,
    DateTimeOffset To,
    long? UserId);

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
            // Projected in the database, so only the columns the DTO needs are read
            var items = query.Select(x => new ItemDto(
                x.Id,
                x.Name,
                x.Type,
                x.LocationId,
                x.Location.Name,
                new List<ItemBookingDto>()));

            return new GetFilteredItemsResponse(await PaginatedList<ItemDto>.CreateAsync(items, request.Page, request.Limit));
        }

        // The day is a Madrid calendar day; Npgsql needs its bounds in UTC
        var day = request.AvailableOnDay.Value;
        var dayStart = Location.FromLocal(day, TimeOnly.MinValue).ToUniversalTime();
        var dayEnd = Location.FromLocal(day.AddDays(1), TimeOnly.MinValue).ToUniversalTime();
        var now = _timeProvider.GetUtcNow();

        var itemsWithBookings = await query
            .Include(x => x.Bookings.Where(b => b.From < dayEnd && b.To > dayStart))
            .ToListAsync();

        var available = itemsWithBookings
            .Where(x => x.HasFreeSlotOn(day, now))
            .Select(x => ToDto(x, x.GetSlotsOn(day).Select(s => new ItemBookingDto(s.From, s.To, s.UserId))));

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

    // Slots are only filled when filtering by day
    private static ItemDto ToDto(Item item, IEnumerable<ItemBookingDto> slots) => new(
        item.Id,
        item.Name,
        item.Type,
        item.LocationId,
        item.Location.Name,
        slots.ToList());
}
