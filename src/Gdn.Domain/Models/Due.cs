using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Represents a generic due (payment deadline). Designed to be reusable across different
/// contexts (invoices, orders, credit notes, etc.) via nullable foreign keys.
/// <para>
/// <see cref="PaidAmount"/> is maintained exclusively by a SQL trigger on the
/// <c>PaymentDues</c> table and must never be written directly by the application.
/// </para>
/// </summary>
public class Due : TrackedEntity<int>
{
    /// <summary>The date by which payment is expected.</summary>
    public DateOnly Date { get; set; }

    /// <summary>The total amount expected for this due.</summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The amount already paid toward this due. Automatically maintained by a
    /// SQL trigger on <c>PaymentDues</c> — do not set from application code.
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>FK to the associated invoice. <see langword="null"/> if not invoice-related.</summary>
    public int? InvoiceId { get; set; }

    /// <summary>Navigation property to the associated invoice.</summary>
    public Invoice? Invoice { get; set; }

    /// <summary>FK to the associated credit note. <see langword="null"/> if not credit-note-related.</summary>
    public int? CreditNoteId { get; set; }

    /// <summary>Navigation property to the associated credit note.</summary>
    public CreditNote? CreditNote { get; set; }

    /// <summary>FK to the associated customer. <see langword="null"/> if not customer-related.</summary>
    public int? CustomerId { get; set; }

    /// <summary>Navigation property to the associated customer.</summary>
    public Customer? Customer { get; set; }

    /// <summary>Payment allocations that partially or fully cover this due.</summary>
    public ICollection<PaymentDue> PaymentDues { get; set; } = [];

    /// <summary>
    /// Returns <see langword="true"/> when <see cref="PaidAmount"/> fully covers <see cref="Amount"/>.
    /// </summary>
    public bool IsPaid => PaidAmount >= Amount;
}
