using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class QuoteRowRepository : Repository<QuoteRow, long>, IQuoteRowRepository
{
    public QuoteRowRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
