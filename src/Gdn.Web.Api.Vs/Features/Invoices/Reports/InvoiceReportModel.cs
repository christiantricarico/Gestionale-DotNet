namespace Gdn.Web.Api.Vs.Features.Invoices.Reports;

internal sealed class InvoiceReportModel
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? CustomerName { get; set; }
    public string? Notes { get; set; }
    public AddressModel SellerAddress { get; set; } = default!;
    public AddressModel CustomerAddress { get; set; } = default!;
    public List<InvoiceRowReportModel> Rows { get; set; } = [];
    public decimal? StampDutyAmount { get; set; }
    public bool StampDutyChargedToCustomer { get; set; }
    public List<DueReportModel> Dues { get; set; } = [];

    /// <summary>The document type label used in the PDF header (e.g., "Fattura" or "Nota di credito").</summary>
    public string DocumentTitle { get; set; } = "Fattura";

    /// <summary>The date field label used in the PDF header (e.g., "Data fattura:" or "Data nota di credito:").</summary>
    public string DateLabel { get; set; } = "Data fattura:";

    /// <summary>The acceptance status label used in the PDF header for quotes (e.g., "NON ACCETTATO" or "ACCETTATO il gg/mm/aaaa"). Left <see langword="null"/> for document types without an acceptance status.</summary>
    public string? AcceptanceStatusLabel { get; set; }
}

internal sealed class InvoiceRowReportModel
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