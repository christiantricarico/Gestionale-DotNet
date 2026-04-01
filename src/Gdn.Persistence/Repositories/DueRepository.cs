using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class DueRepository : Repository<Due, int>, IDueRepository
{
    public DueRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
