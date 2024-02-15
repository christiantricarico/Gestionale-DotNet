using Gdn.Domain.Data;
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

    private void ChangeTracker_StateChanged(object? sender, EntityStateChangedEventArgs e)
    {
        throw new NotImplementedException();
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
        //var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        //var principal = httpContextAccessor?.HttpContext?.User;

        //if (e.Entry.Entity is TrackedEntity<int> trackedEntity)
        //{
        //    switch (e.Entry.State)
        //    {
        //        case EntityState.Modified:
        //            trackedEntity.UpdatedAt = DateTime.UtcNow;
        //            trackedEntity.UpdatedBy = principal?.Identity?.Name;
        //            break;
        //        case EntityState.Added:
        //            trackedEntity.CreatedAt = DateTime.UtcNow;
        //            trackedEntity.CreatedBy = principal?.Identity?.Name;
        //            break;
        //    }
        //}
    }
}
