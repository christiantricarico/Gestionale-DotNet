using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.Customers;

public class CustomerViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Codice fiscale")]
    public string? FiscalCode { get; set; }

    [Display(Name = "Partita iva")]
    public string? VatNumber { get; set; }
}
