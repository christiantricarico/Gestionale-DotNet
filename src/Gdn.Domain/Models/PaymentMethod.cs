using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Represents a payment method that can be associated with a payment.
/// The <see cref="DigitalInvoiceCode"/> property maps to the Agenzia delle Entrate
/// payment modality codes (MP01-MP23) used in the digital invoice XML (FatturaElettronica).
/// </summary>
public class PaymentMethod : RegistryEntity<int>
{
    /// <summary>
    /// The Agenzia delle Entrate payment modality code (e.g. "MP01" for cash, "MP05" for bank transfer)
    /// used to populate the ModalitaPagamento field in the digital invoice XML.
    /// </summary>
    public string? DigitalInvoiceCode { get; set; }
}
