using BookingSystem.Domain.Entities;

namespace BookingSystem.Domain.Contracts.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> Query { get; }
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
