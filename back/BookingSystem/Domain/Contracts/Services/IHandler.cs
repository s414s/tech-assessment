namespace BookingSystem.Domain.Contracts.Services;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}
