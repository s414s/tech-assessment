namespace BookingSystem.Domain.Entities;

public class User : Entity
{
    public required string Name { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = [];
}

