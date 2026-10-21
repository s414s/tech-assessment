using BookingSystem.Infrastructure.Persistence.Repositories;

namespace BookingSystem.Presentation.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/login", async (IUsersRepository repository) =>
        {
            var users = await repository.GetByIdAndPwdAsync(1, "1111");
            return Results.Ok(users);
        });

        return group;
    }
}
