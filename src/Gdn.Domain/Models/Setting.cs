using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Generic store for application settings. One row per settings section, where <see cref="Value"/>
/// holds the JSON payload of that whole section, so adding a new setting later needs neither a new
/// column nor a migration.
/// </summary>
public class Setting : TrackedEntity<int>
{
    /// <summary>The section key, declared by the settings section type itself.</summary>
    public string Key { get; set; } = default!;

    /// <summary>The JSON serialized section.</summary>
    public string Value { get; set; } = default!;
}
