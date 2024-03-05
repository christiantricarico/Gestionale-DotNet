using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class ProductCategoryRepository : Repository<ProductCategory, int>, IProductCategoryRepository
{
    public ProductCategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
