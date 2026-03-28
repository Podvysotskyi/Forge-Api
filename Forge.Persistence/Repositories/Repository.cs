using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Forge.Persistence.Repositories;

public abstract class Repository<TEntity>(RepositoryDbContext dbContext)
    where TEntity : class
{
    protected RepositoryDbContext DbContext { get; } = dbContext;

    protected DbSet<TEntity> Entities => DbContext.Set<TEntity>();

    protected EntityEntry<TEntry> Entry<TEntry>(TEntry entity)
        where TEntry : class
    {
        return DbContext.Entry(entity);
    }
}
