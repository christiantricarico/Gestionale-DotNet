using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.PaymentMethods;

public class PaymentMethodInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Codice richiesto.")]
    [StringLength(10)]
    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [StringLength(4)]
    [Display(Name = "Codice fattura elettronica")]
    public string? DigitalInvoiceCode { get; set; }
}
