using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.PaymentMethods;

public class PaymentMethodViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Codice fattura elettronica")]
    public string? DigitalInvoiceCode { get; set; }
}
