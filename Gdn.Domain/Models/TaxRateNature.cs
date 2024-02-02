using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class TaxRateNature : TrackedEntity<int>
{
    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TaxRate> TaxRates { get; set; } = new List<TaxRate>();
}
