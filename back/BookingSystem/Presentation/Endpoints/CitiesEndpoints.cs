using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Presentation.Endpoints;

public static class CitiesEndpoints
{
    public static RouteGroupBuilder MapCitiesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cities")
            .WithTags("Cities");

        group.MapGet("/", async (IRepository<City> repository) =>
        {
            var cities = await repository.Query.ToListAsync();

            return Results.Ok(cities);
        });

        return group;
    }
}
