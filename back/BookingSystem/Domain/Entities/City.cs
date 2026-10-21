namespace BookingSystem.Domain.Entities;

public class City : Entity
{
    public required string Name { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = [];
}

