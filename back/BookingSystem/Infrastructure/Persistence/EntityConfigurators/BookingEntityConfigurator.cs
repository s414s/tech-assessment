using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.EntityConfigurators;

internal class BookingEntityConfigurator : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.From).IsRequired();
        builder.Property(x => x.To).IsRequired();

        builder
            .HasOne(m => m.Item)
            .WithMany(d => d.Bookings)
            .HasForeignKey(m => m.ItemId)
            .IsRequired();

        builder
            .HasOne(m => m.User)
            .WithMany(g => g.Bookings)
            .HasForeignKey(m => m.UserId)
            .IsRequired();
    }
}

