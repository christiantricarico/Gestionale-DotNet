namespace Gdn.Domain.Models.Bases;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }
}

public class SoftDeletableEntity<T> : BaseEntity<T>, ISoftDeletableEntity
    where T : struct
{
    public bool IsDeleted { get; set; }
}
