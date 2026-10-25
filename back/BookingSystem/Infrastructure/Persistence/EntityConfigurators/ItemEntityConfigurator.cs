using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.EntityConfigurators;

internal class ItemEntityConfigurator : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder
            .HasOne(m => m.Location)
            .WithMany(d => d.Items)
            .HasForeignKey(m => m.LocationId)
            .IsRequired();

        builder.HasData(
            // Center (LocationId = 1)
            new Item { Id = 1, Name = "Center Printer 1", Type = ItemType.Printer, LocationId = 1 },
            new Item { Id = 2, Name = "Center Computer 1", Type = ItemType.Computer, LocationId = 1 },
            new Item { Id = 3, Name = "Center Computer 2", Type = ItemType.Computer, LocationId = 1 },
            new Item { Id = 4, Name = "Center Monitor 1", Type = ItemType.Monitor, LocationId = 1 },
            new Item { Id = 5, Name = "Center Projector 1", Type = ItemType.Projector, LocationId = 1 },

            // Sol (LocationId = 2)
            new Item { Id = 6, Name = "Sol Printer 1", Type = ItemType.Printer, LocationId = 2 },
            new Item { Id = 7, Name = "Sol Printer 2", Type = ItemType.Printer, LocationId = 2 },
            new Item { Id = 8, Name = "Sol Computer 1", Type = ItemType.Computer, LocationId = 2 },
            new Item { Id = 9, Name = "Sol Monitor 1", Type = ItemType.Monitor, LocationId = 2 },
            new Item { Id = 10, Name = "Sol Projector 1", Type = ItemType.Projector, LocationId = 2 });
    }
}

