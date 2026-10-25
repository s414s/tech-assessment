using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.EntityConfigurators;

internal class LocationEntityConfigurator : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder
            .HasMany(m => m.Items)
            .WithOne(d => d.Location)
            .HasForeignKey(d => d.LocationId)
            .IsRequired();

        builder.HasData(
            new Location { Id = 1, Name = "Center" },
            new Location { Id = 2, Name = "Sol" });
    }
}

