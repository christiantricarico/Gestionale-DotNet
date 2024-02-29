using Gdn.Domain.Data;
using Gdn.Domain.Models.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Gdn.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(AppDbContext dbContext,
                      IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _dbContext.ChangeTracker.StateChanged += UpdateTrackingData;
        _dbContext.ChangeTracker.Tracked += UpdateTrackingData;

        _serviceProvider = serviceProvider;
    }

    public TRepository GetRepository<TRepository>()
        where TRepository : class
    {
        var repo = _serviceProvider.GetRequiredService<TRepository>();
        return repo;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private void UpdateTrackingData(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is ITrackedEntity trackedEntity)
        {
            switch (e.Entry.State)
            {
                case EntityState.Modified:
                    trackedEntity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Added:
                    trackedEntity.CreatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
