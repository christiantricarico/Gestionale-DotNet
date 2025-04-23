using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class InvoiceRowRepository : Repository<InvoiceRow, long>, IInvoiceRowRepository
{
    public InvoiceRowRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}