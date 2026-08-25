using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class QuoteRepository : Repository<Quote, int>, IQuoteRepository
{
    public QuoteRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
