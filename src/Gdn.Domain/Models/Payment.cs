using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Represents a payment made by a customer. A single payment can be allocated
/// across multiple dues via <see cref="PaymentDues"/>.
/// </summary>
/// <remarks>
/// Extends <see cref="TrackedEntity{T}"/> to support audit tracking (<c>CreatedAt</c>, <c>UpdatedAt</c>)
/// and future soft-delete capability (<c>IsDeleted</c>), even though soft-delete is not enforced today.
/// </remarks>
public class Payment : TrackedEntity<int>
{
    /// <summary>The date on which the payment was made.</summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// The signed total amount of this payment.
    /// Positive amounts identify incoming customer receipts, while negative amounts
    /// identify outgoing customer payments such as credit-note reimbursements.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>FK to the payment method used. <see langword="null"/> if not specified.</summary>
    public int? PaymentMethodId { get; set; }

    /// <summary>Navigation property to the payment method used.</summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// FK to the customer associated with this payment. Set when registering a customer-level
    /// payment so the movement can be queried directly by customer.
    /// <see langword="null"/> for invoice-level payments registered via the invoice workflow.
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>Navigation property to the customer who made this payment.</summary>
    public Customer? Customer { get; set; }

    /// <summary>Allocations of this payment's amount to individual dues.</summary>
    public ICollection<PaymentDue> PaymentDues { get; set; } = [];
}
