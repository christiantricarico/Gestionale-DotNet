using Gdn.Presentation.Shared.Models.Invoices;
using Gdn.Presentation.Shared.Models.MeasurementUnits;
using Gdn.Presentation.Shared.Models.TaxRates;

namespace Gdn.Web.Fluentblazor.Components.Invoices;

public class EditInvoiceRowDataSet
{
    public InvoiceRowInputModel? Input { get; set; }
    public TaxRateViewModel? SelectedTaxRate { get; set; }
    public MeasurementUnitViewModel? SelectedMeasurementUnit { get; set; }
}
