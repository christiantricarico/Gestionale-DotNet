namespace Gdn.Domain.Data;

public interface IUnitOfWork
{
    TRepository GetRepository<TRepository>()
            where TRepository : class;

    Task SaveChangesAsync();
}
