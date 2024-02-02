namespace Gdn.Domain.Models.Bases;

public class TrackedSoftDeletableEntity<T> : BaseEntity<T>, ITrackedEntity, ISoftDeletableEntity
    where T : struct
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = default!;
    public bool IsDeleted { get; set; }
}
