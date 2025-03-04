using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class TaxRateRepository : Repository<TaxRate, int>, ITaxRateRepository
{
    public TaxRateRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
