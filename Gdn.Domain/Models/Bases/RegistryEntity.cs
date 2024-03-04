namespace Gdn.Domain.Models.Bases;

public class RegistryEntity<TKey> : TrackedEntity<TKey>
    where TKey : struct
{
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
}
