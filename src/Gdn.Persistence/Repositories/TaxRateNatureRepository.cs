using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class TaxRateNatureRepository : Repository<TaxRateNature, int>, ITaxRateNatureRepository
{
    public TaxRateNatureRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
