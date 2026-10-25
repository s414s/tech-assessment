using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Infrastructure.Persistence.Context;
using BookingSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(options => options
            .UseNpgsql(connectionString)
            .UseSeeding((context, _) => BookingSeeder.Seed(context))
            .UseAsyncSeeding((context, _, cancellationToken) => BookingSeeder.SeedAsync(context, cancellationToken)));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}

