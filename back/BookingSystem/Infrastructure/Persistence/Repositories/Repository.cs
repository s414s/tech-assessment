using BookingSystem.Domain.Contracts.Repositories;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(DatabaseContext dbContext) : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DatabaseContext _dbContext = dbContext;
    protected readonly DbSet<TEntity> _set = dbContext.Set<TEntity>();

    public virtual IQueryable<TEntity> Query => _set;

    public virtual void Add(TEntity entity) => _set.Add(entity);

    public virtual void Update(TEntity entity) => _set.Update(entity);

    public virtual void Remove(TEntity entity) => _set.Remove(entity);

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
