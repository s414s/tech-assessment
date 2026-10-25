using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Contracts.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> Query { get; }
    Task<long> AddAndSaveChangesAsync(TEntity entity, CancellationToken cancellationToken = default);
}
