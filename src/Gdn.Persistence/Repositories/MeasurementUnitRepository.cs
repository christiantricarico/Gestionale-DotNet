using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class MeasurementUnitRepository : Repository<MeasurementUnit, int>, IMeasurementUnitRepository
{
    public MeasurementUnitRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
