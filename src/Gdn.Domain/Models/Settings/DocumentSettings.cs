namespace Gdn.Domain.Models.Settings;

/// <summary>
/// Settings shared by the documents (Fattura, Nota di credito, Preventivo, Rapporto di intervento)
/// editable by administrators. Currently holds only the default tax rate, but is the intended home
/// for future document-wide settings rather than a tax-rate-specific section.
/// </summary>
public class DocumentSettings : ISettingsSection
{
    /// <inheritdoc />
    public static string Key => "Documents";

    /// <summary>
    /// Tax rate applied by default to a document row when neither the row's product/service nor its
    /// customer specify one. <see langword="null"/> when no system-wide default is configured, in
    /// which case the row is created without a tax rate and the user must pick one manually.
    /// </summary>
    public int? DefaultTaxRateId { get; set; }
}
