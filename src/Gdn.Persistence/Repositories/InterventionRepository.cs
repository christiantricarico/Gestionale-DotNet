using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class InterventionRepository : Repository<Intervention, int>, IInterventionRepository
{
    public InterventionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
