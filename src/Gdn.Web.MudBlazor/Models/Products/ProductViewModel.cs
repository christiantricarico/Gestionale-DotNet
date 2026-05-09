using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Products;

public class ProductViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Tipo")]
    public string Type { get; set; } = default!;

    public int? MeasurementUnitId { get; set; }

    [Display(Name = "Unità di misura")]
    public string? MeasurementUnitName { get; set; }

    public int? TaxRateId { get; set; }

    [Display(Name = "Aliquota IVA")]
    public string? TaxRateCode { get; set; }

    public decimal? TaxRateValue { get; set; }

    public int? ProductCategoryId { get; set; }

    [Display(Name = "Categoria")]
    public string? ProductCategoryName { get; set; }
}

