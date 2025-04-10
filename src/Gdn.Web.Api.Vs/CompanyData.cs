namespace Gdn.Web.Api.Vs;

public class CompanyData
{
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? VatNumber { get; set; }
    public string? FiscalCode { get; set; }
    public FatturaElettronicaData? FatturaElettronicaData { get; set; }
}

public class FatturaElettronicaData
{
    public string? CodiceDestinatario { get; set; }
    public string? PECDestinatario { get; set; }

    public string? IdPaeseTrasmittente { get; set; }
    public string? IdCodiceTrasmittente { get; set; }
    public string? TelefonoTrasmittente { get; set; }
    public string? EmailTrasmittente { get; set; }

    public string? IndirizzoSedeCedente { get; set; }
    public string? CapSedeCedente { get; set; }
    public string? ComuneSedeCedente { get; set; }
    public string? ProvinciaSedeCedente { get; set; }
    public string? NazioneSedeCedente { get; set; }
    public string? RegimeFiscaleCedente { get; set; }
}