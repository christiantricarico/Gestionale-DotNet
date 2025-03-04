using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Data.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : struct
{
    TEntity Add(TEntity entity);
    Task RemoveAsync(TId id);
    Task<TEntity?> GetAsync(TId id);
    Task<TEntity?> GetAsync(TId id, IEnumerable<string> includes);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> includes);
    Task<IEnumerable<TEntity>> GetAllAsync(Func<TEntity, bool>? predicate = null, IEnumerable<string>? includes = null);
    void Update(TEntity entity);
}
