namespace Gdn.Domain.Models.Settings;

/// <summary>
/// Marks a plain class as a settings section. The static abstract member binds the storage key to
/// the type, so callers never have to pass a loose string around.
/// </summary>
public interface ISettingsSection
{
    /// <summary>The key under which the section is stored.</summary>
    static abstract string Key { get; }
}
