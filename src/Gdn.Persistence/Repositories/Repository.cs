using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Bases;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence.Repositories;

internal abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    private readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TEntity Add(TEntity entity)
    {
        var result = _dbContext.Set<TEntity>().Add(entity);
        return result.Entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var data = await _dbContext.Set<TEntity>().ToListAsync();
        return data;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Func<TEntity, bool>? predicate = null, IEnumerable<string>? includes = null)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        if (predicate != null)
            query = query.Where(predicate).AsQueryable();

        var data = query.OrderBy(e => e.Id).ToList();
        return await Task.FromResult(data);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> includes)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        if (includes is not null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        var data = await query.ToListAsync();
        return data;
    }

    public async Task<TEntity?> GetAsync(TId id)
    {
        var entity = await _dbContext.FindAsync<TEntity>(id);
        return entity;
    }

    public async Task<TEntity?> GetAsync(TId id, IEnumerable<string> includes)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        if (includes is not null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        var entity = await query
            .Where(e => e.Id.Equals(id))
            .SingleOrDefaultAsync();

        return entity;
    }

    public async Task RemoveAsync(TId id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);

        if (entity is not null)
            _dbContext.Set<TEntity>().Remove(entity);
    }

    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }
}
