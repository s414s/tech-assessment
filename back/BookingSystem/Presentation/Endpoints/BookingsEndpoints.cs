
using BookingSystem.Application.Handlers.Bookings;
using BookingSystem.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Presentation.Endpoints;

public static class BookingsEndpoints
{
    public static RouteGroupBuilder MapBookingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings")
            .WithTags("Bookings");

        group.MapGet("/user/{userId:long}", async (
            long userId,
            IHandler<GetFilteredBookingsQuery, GetFilteredBookingsResponse> handler,
            int page = 1,
            int limit = 10) =>
        {
            var response = await handler.HandleAsync(new GetFilteredBookingsQuery(userId, page, limit));
            return Results.Ok(response);
        });

        group.MapGet("/item/{itemId:long}", async (
            long itemId,
            DateTimeOffset from,
            DateTimeOffset to,
            IHandler<GetItemBookingsQuery, GetItemBookingsResponse> handler) =>
        {
            var response = await handler.HandleAsync(new GetItemBookingsQuery(from, to, itemId));
            return Results.Ok(response);
        });

        group.MapPost("/", async (
            [FromBody] CreateBookingCommand command,
            IHandler<CreateBookingCommand, CreateBookingResponse> handler) =>
        {
            var response = await handler.HandleAsync(command);
            return Results.Created($"/api/bookings/{response.Id}", response);
        });

        return group;
    }
}
