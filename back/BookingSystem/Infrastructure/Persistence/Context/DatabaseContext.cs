using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence.EntityConfigurators;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfigurator());
        modelBuilder.ApplyConfiguration(new BookingEntityConfigurator());
        modelBuilder.ApplyConfiguration(new LocationEntityConfigurator());
        modelBuilder.ApplyConfiguration(new ItemEntityConfigurator());
    }
}

