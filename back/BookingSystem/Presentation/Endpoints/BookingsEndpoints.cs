
using BookingSystem.Application.Handlers.Bookings;
using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Presentation.Endpoints;

public static class BookingsEndpoints
{
    public static RouteGroupBuilder MapBookingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings")
            .WithTags("Bookings");

        // Get bookings for a specific item
        group.MapGet("/item/{itemId:long}", async (long itemId, [AsParameters] GetItemBookingsQuery @params) =>
        {
            var query = @params with { ItemId = itemId };

            throw new NotImplementedException();

            return Results.Ok();
        });

        // Get my bookings
        group.MapGet("/all", async (IRepository<Booking> repository) =>
        {
            throw new NotImplementedException();

            return Results.Ok();
        });

        // Create a booking
        group.MapPost("/", async ([FromBody] CreateBookingCommand command) =>
        {

            throw new NotImplementedException();

            return Results.Created();
        });

        return group;
    }
}
