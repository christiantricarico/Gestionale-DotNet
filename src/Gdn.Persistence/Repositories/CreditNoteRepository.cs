using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class CreditNoteRepository : Repository<CreditNote, int>, ICreditNoteRepository
{
    public CreditNoteRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
