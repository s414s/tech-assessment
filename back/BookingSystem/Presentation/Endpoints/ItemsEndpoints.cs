using BookingSystem.Application.Handlers.Items;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Domain.Enums;

namespace BookingSystem.Presentation.Endpoints;

public static class ItemsEndpoints
{
    public static RouteGroupBuilder MapItemsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items")
            .WithTags("Items");

        group.MapGet("/", async (
            ItemType? type,
            long? locationId,
            DateOnly? availableOnDate,
            IHandler<GetFilteredItemsQuery, GetFilteredItemsResponse> handler,
            int page = 1,
            int limit = 10) =>
        {
            var response = await handler.HandleAsync(new GetFilteredItemsQuery(type, locationId, availableOnDate, page, limit));

            return Results.Ok(response);
        });

        group.MapGet("/types", () =>
        {
            var types = Enum.GetValues<ItemType>()
                .Select(x => new { Id = (int)x, Name = x.ToString() });

            return Results.Ok(types);
        });

        return group;
    }
}
