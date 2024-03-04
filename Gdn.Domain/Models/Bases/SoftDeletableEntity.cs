namespace Gdn.Domain.Models.Bases;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }
}

public class SoftDeletableEntity<TKey> : BaseEntity<TKey>, ISoftDeletableEntity
    where TKey : struct
{
    public bool IsDeleted { get; set; }
}
