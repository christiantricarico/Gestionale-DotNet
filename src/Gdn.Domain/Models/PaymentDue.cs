using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Join entity representing the many-to-many relationship between <see cref="Payment"/>
/// and <see cref="Due"/>. Each record allocates a portion (or all) of a payment to a due.
/// <para>
/// Inserting or deleting a <see cref="PaymentDue"/> automatically updates
/// <see cref="Due.PaidAmount"/> via a SQL trigger on this table.
/// </para>
/// </summary>
public class PaymentDue : BaseEntity<int>
{
    /// <summary>FK to the due being (partially) covered by this allocation.</summary>
    public int DueId { get; set; }

    /// <summary>Navigation property to the due being covered.</summary>
    public Due Due { get; set; } = default!;

    /// <summary>FK to the payment providing the funds.</summary>
    public int PaymentId { get; set; }

    /// <summary>Navigation property to the payment providing the funds.</summary>
    public Payment Payment { get; set; } = default!;

    /// <summary>
    /// The portion of the payment's amount allocated to <see cref="Due"/>.
    /// May partially or fully cover the due's outstanding balance.
    /// </summary>
    public decimal Amount { get; set; }
}
