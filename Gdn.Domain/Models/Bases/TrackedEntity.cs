using System.ComponentModel.DataAnnotations;

namespace Gdn.Domain.Models.Bases;

public interface ITrackedEntity
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
    byte[] RowVersion { get; set; }
}

public class TrackedEntity<T> : BaseEntity<T>, ITrackedEntity
    where T : struct
{
    public DateTime CreatedAt { get; set; }

    [MaxLength(255)]
    public string? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(255)]
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
