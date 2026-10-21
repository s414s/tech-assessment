namespace BookingSystem.Domain.Exceptions;

public class DomainException : Exception
{

}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base("Entity could not be found.") { }
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

