using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class InterventionReportRepository : Repository<InterventionReport, int>, IInterventionReportRepository
{
    public InterventionReportRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
