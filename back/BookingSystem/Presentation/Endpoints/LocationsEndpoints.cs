using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Presentation.Endpoints;

public static class LocationsEndpoints
{
    public static RouteGroupBuilder MapLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations")
            .WithTags("Locations");

        group.MapGet("/", async (IRepository<Location> repository) =>
        {
            var locations = await repository.Query.ToListAsync();
            return Results.Ok(locations);
        });

        return group;
    }
}
