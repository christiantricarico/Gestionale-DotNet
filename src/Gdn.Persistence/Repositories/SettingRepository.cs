using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence.Repositories;

internal sealed class SettingRepository : Repository<Setting, int>, ISettingRepository
{
    public SettingRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc />
    public Task<Setting?> GetByKeyAsync(string key)
        => _dbContext.Settings.SingleOrDefaultAsync(e => e.Key == key);
}
