using Gdn.Domain.Models.Bases;
using Gdn.Domain.Models.Enums;

namespace Gdn.Domain.Models;

public class Product : TrackedEntity<int>
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public string Type { get; set; } = ProductType.Product;

    public int? ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }

    public int? MeasurementUnitId { get; set; }
    public MeasurementUnit? MeasurementUnit { get; set; }

    public int? TaxRateId { get; set; }
    public TaxRate? TaxRate { get; set; }
}
