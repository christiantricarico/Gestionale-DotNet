using Gdn.Domain.Models;

namespace Gdn.Domain.Data.Repositories;

/// <summary>Generic settings repository, addressed by section key.</summary>
public interface ISettingRepository : IRepository<Setting, int>
{
    /// <summary>Returns the stored section, or <see langword="null"/> when it has never been saved.</summary>
    Task<Setting?> GetByKeyAsync(string key);
}
