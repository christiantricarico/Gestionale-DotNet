using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class TaxRateNature : SoftDeletableEntity<int>
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}
