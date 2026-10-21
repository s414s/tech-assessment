namespace BookingSystem.Domain.Entities;

public class User : Entity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; } // just for demo purposes, in real life we would use a hashed password

    public virtual ICollection<Booking> Bookings { get; set; } = [];
}

