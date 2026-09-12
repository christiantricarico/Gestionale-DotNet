using System.Text.Json;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Gdn.Web.Api.Vs.Features.Settings;

/// <summary>
/// Reads and writes settings sections on top of the generic setting store, keeping callers typed.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Returns the stored section, or a new instance carrying the property defaults when the section
    /// has never been saved.
    /// </summary>
    Task<TSection> GetAsync<TSection>() where TSection : class, ISettingsSection, new();

    /// <summary>Persists the section and invalidates the cached copy.</summary>
    Task SaveAsync<TSection>(TSection section) where TSection : class, ISettingsSection, new();
}

/// <inheritdoc />
public sealed class SettingsService(ISettingRepository settingRepository, IUnitOfWork unitOfWork, IMemoryCache cache) : ISettingsService
{
    // Settings are read on every sign in and every token refresh, so a short cache keeps those
    // paths from hitting the database each time.
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<TSection> GetAsync<TSection>() where TSection : class, ISettingsSection, new()
    {
        if (cache.TryGetValue(CacheKeyOf<TSection>(), out TSection? cached) && cached is not null)
        {
            return cached;
        }

        var stored = await settingRepository.GetByKeyAsync(TSection.Key);

        var section = stored is null
            ? new TSection()
            : JsonSerializer.Deserialize<TSection>(stored.Value) ?? new TSection();

        cache.Set(CacheKeyOf<TSection>(), section, CacheDuration);

        return section;
    }

    /// <inheritdoc />
    public async Task SaveAsync<TSection>(TSection section) where TSection : class, ISettingsSection, new()
    {
        var payload = JsonSerializer.Serialize(section);
        var stored = await settingRepository.GetByKeyAsync(TSection.Key);

        if (stored is null)
        {
            settingRepository.Add(new Setting { Key = TSection.Key, Value = payload });
        }
        else
        {
            stored.Value = payload;
            settingRepository.Update(stored);
        }

        await unitOfWork.SaveChangesAsync();

        cache.Remove(CacheKeyOf<TSection>());
    }

    private static string CacheKeyOf<TSection>() where TSection : ISettingsSection
        => $"settings:{TSection.Key}";
}
