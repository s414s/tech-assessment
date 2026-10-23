using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence.Context;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DatabaseContext _dbContext;

    public virtual IQueryable<TEntity> Query => _dbContext.Set<TEntity>();

    public Repository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<long> AddAndSaveChangesAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
