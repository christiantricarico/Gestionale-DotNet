namespace Gdn.Domain.Models.Bases;

public interface ITrackedEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public class TrackedEntity<T> : BaseEntity<T>, ITrackedEntity, ISoftDeletableEntity
    where T : struct
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
