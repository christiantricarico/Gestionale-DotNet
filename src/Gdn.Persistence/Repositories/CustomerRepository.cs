using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class CustomerRepository : Repository<Customer, int>, ICustomerRepository
{
    public CustomerRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
