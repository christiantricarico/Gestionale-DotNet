using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class CreditNoteRowRepository : Repository<CreditNoteRow, long>, ICreditNoteRowRepository
{
    public CreditNoteRowRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
