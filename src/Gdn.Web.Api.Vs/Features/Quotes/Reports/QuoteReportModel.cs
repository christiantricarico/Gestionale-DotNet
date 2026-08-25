namespace Gdn.Web.Api.Vs.Features.Quotes.Reports;

internal sealed class QuoteReportModel
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? CustomerName { get; set; }
    public AddressModel SellerAddress { get; set; } = default!;
    public AddressModel CustomerAddress { get; set; } = default!;
    public List<QuoteRowReportModel> Rows { get; set; } = [];

    /// <summary>The acceptance status label used in the PDF header (e.g., "NON ACCETTATO" or "ACCETTATO il gg/mm/aaaa").</summary>
    public string? AcceptanceStatusLabel { get; set; }
}

internal sealed class QuoteRowReportModel
{
    public string? Description { get; set; }
    public string? MeasurementUnitCode { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TaxRate { get; set; }
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
