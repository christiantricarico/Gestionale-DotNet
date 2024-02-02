using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class TaxRate : TrackedSoftDeletableEntity<int>
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}
