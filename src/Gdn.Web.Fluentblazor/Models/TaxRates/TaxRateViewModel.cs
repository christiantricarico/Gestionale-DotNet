using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.TaxRates;

public class TaxRateViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Aliquota")]
    public decimal Rate { get; set; }

    public int? TaxRateNatureId { get; set; }

    [Display(Name = "Natura Iva")]
    public string? TaxRateNatureName { get; set; }
}
