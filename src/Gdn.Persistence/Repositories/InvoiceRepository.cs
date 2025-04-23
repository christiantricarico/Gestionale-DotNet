using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class InvoiceRepository : Repository<Invoice, int>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
