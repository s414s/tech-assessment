namespace BookingSystem.Domain.Entities;

public class Location : Entity
{
    public required string Name { get; set; }

    public virtual ICollection<Item> Items { get; set; } = [];
}

