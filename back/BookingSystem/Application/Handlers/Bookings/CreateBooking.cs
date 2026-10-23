using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Application.Handlers.Bookings;

public sealed record CreateBookingCommand(DateTimeOffset From, DateTimeOffset To, long ItemId, long UserId);
public sealed record CreateBookingResponse(long Id);

public class CreateBookingHandler : IHandler<CreateBookingCommand, CreateBookingResponse>
{
    private readonly IRepository<Booking> _bookingsRepository;
    private readonly IRepository<Item> _itemsRepository;
    private readonly IRepository<User> _usersRepository;
    private readonly TimeProvider _timeProvider;

    public CreateBookingHandler(
        IRepository<Booking> bookingsRepository,
        IRepository<Item> itemsRepository,
        IRepository<User> usersRepository,
        TimeProvider timeProvider)
    {
        _bookingsRepository = bookingsRepository;
        _itemsRepository = itemsRepository;
        _usersRepository = usersRepository;
        _timeProvider = timeProvider;
    }

    public async Task<CreateBookingResponse> HandleAsync(CreateBookingCommand request)
    {
        var user = await _usersRepository.Query.FirstOrDefaultAsync(x => x.Id == request.UserId)
            ?? throw new DomainException($"User {request.UserId} does not exist.");

        var item = await _itemsRepository.Query
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == request.ItemId)
            ?? throw new DomainException($"Item {request.ItemId} does not exist.");

        // Create first so the overlap check uses the rounded UTC times that will be stored
        var booking = Booking.Create(request.From, request.To, user, item, _timeProvider.GetUtcNow());

        var overlaps = await _bookingsRepository.Query
            .AnyAsync(x => x.ItemId == item.Id && x.From < booking.To && x.To > booking.From);

        if (overlaps)
            throw new DomainException("The item is already booked during the requested time.");

        var id = await _bookingsRepository.AddAndSaveChangesAsync(booking);

        return new CreateBookingResponse(id);
    }
}
