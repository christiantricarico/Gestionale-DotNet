namespace Gdn.Web.Api.Vs.Features.CreditNotes.Reports;

internal sealed class CreditNoteReportModel
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? CustomerName { get; set; }
    public AddressModel SellerAddress { get; set; } = default!;
    public AddressModel CustomerAddress { get; set; } = default!;
    public List<CreditNoteRowReportModel> Rows { get; set; } = [];
    public decimal? StampDutyAmount { get; set; }
    public bool StampDutyChargedToCustomer { get; set; }
    public List<DueReportModel> Dues { get; set; } = [];
}

internal sealed class CreditNoteRowReportModel
{
    public string? Description { get; set; }
    public string? MeasurementUnitCode { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TaxRate { get; set; }
}

internal sealed class DueReportModel
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
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
