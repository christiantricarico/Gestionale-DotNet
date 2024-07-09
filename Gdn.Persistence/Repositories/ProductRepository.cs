using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class ProductRepository : Repository<Product, int>, IProductRepository
{
    public ProductRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
