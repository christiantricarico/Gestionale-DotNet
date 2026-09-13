using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class InterventionRowRepository : Repository<InterventionRow, long>, IInterventionRowRepository
{
    public InterventionRowRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
