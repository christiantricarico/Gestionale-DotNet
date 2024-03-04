using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public class TaxRate : RegistryEntity<int>
{
    public int? TaxRateNatureId { get; set; }
    public TaxRateNature? TaxRateNature { get; set; }
}
