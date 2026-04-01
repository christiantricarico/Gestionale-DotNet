using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;

namespace Gdn.Persistence.Repositories;

internal sealed class PaymentMethodRepository : Repository<PaymentMethod, int>, IPaymentMethodRepository
{
    public PaymentMethodRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
