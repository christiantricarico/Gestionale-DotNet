namespace Gdn.Web.Api.Vs.Features.Payments.Reports;

internal sealed class ReceiptReportModel
{
    public int PaymentId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethodName { get; set; }
    public AddressModel SellerAddress { get; set; } = default!;
    public AddressModel CustomerAddress { get; set; } = default!;
    public List<ReceiptDueRowModel> CoveredDues { get; set; } = [];
}

internal sealed class ReceiptDueRowModel
{
    public DateOnly DueDate { get; set; }
    public string? DocumentNumber { get; set; }
    public string DocumentType { get; set; } = default!;
    public decimal DueAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
}

internal sealed class AddressModel
{
    public string? CompanyName { get; set; }
    public string? PostalCode { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
