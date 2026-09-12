namespace Gdn.Web.MudBlazor.Models.Dashboard;

public class DashboardViewModel
{
    /// <summary>
    /// The counts restricted to administrators are nullable because the server leaves them out for a
    /// coworker, rather than sending them and trusting the interface to hide them.
    /// </summary>
    public int? CustomerCount { get; set; }

    public int? CurrentYearInvoiceCount { get; set; }

    public int? CurrentYearCreditNoteCount { get; set; }

    public int CurrentYearInterventionCount { get; set; }
}
