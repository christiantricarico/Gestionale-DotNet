using FatturaElettronica.Defaults;
using FatturaElettronica.Extensions;
using FatturaElettronica.Ordinaria;
using FatturaElettronica.Ordinaria.FatturaElettronicaBody;
using FatturaElettronica.Ordinaria.FatturaElettronicaHeader;
using FluentValidation.Results;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Gdn.Web.Api.Vs.Features.Invoices.Xml;

public class InvoiceXmlGenerator(
    IOptions<AppSettings> appSettings,
    IInvoiceRepository invoiceRepository,
    ITaxRateNatureRepository taxRateNatureRepository)
{
    public async Task<Stream> GenerateXmlStream(int invoiceId)
    {
        Invoice? invoice = await invoiceRepository.GetAsync(invoiceId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit", "Dues.PaymentDues.Payment.PaymentMethod"]);
        if (invoice == null)
            throw new InvalidOperationException($"Invoice with ID {invoiceId} not found.");

        FatturaOrdinaria fatturaOrdinaria = await MapInvoiceToFatturaOrdinariaAsync(invoice);
        var stream = CreateXmlStream(fatturaOrdinaria);

        return stream;
    }

    private async Task<FatturaOrdinaria> MapInvoiceToFatturaOrdinariaAsync(Invoice invoice)
    {
        Instance instanceType = Instance.Privati;

        var fattura = FatturaOrdinaria.CreateInstance(instanceType);

        #region FatturaElettronicaHeader

        var companyData = appSettings.Value.CompanyData;
        var header = fattura.FatturaElettronicaHeader;

        header.DatiTrasmissione.IdTrasmittente.IdPaese = companyData.FatturaElettronicaData?.IdPaeseTrasmittente;
        header.DatiTrasmissione.IdTrasmittente.IdCodice = companyData.FatturaElettronicaData?.IdCodiceTrasmittente;
        header.DatiTrasmissione.ProgressivoInvio = $"FA{invoice.Date.Year.ToString().Substring(2, 2)}{invoice.Number.PadLeft(6, '0')}";

        header.DatiTrasmissione.CodiceDestinatario = invoice.Customer.Sdi;

        header.DatiTrasmissione.ContattiTrasmittente.Telefono = companyData.FatturaElettronicaData?.TelefonoTrasmittente;
        header.DatiTrasmissione.ContattiTrasmittente.Email = companyData.FatturaElettronicaData?.EmailTrasmittente;

        header.DatiTrasmissione.PECDestinatario = invoice.Customer.Pec;

        SetCedentePrestatore(header);
        SetCessionarioCommittente(header, invoice.Customer);

        #endregion

        #region FatturaElettronicaBody

        var body = new FatturaElettronicaBody();
        body.DatiGenerali.DatiGeneraliDocumento.Numero = invoice.Number;
        body.DatiGenerali.DatiGeneraliDocumento.Data = invoice.Date.ToDateTime(TimeOnly.MinValue);
        body.DatiGenerali.DatiGeneraliDocumento.TipoDocumento = "TD01"; // fattura immediata
        body.DatiGenerali.DatiGeneraliDocumento.Divisa = "EUR";

        int indexNumeroLinea = 0;

        //Dettaglio linee - MERCE
        foreach (var item in invoice.Rows)
        {
            var dettaglioLinea = new FatturaElettronica.Ordinaria.FatturaElettronicaBody.DatiBeniServizi.DettaglioLinee()
            {
                NumeroLinea = ++indexNumeroLinea,
                Descrizione = item.Description,
            };

            if (item.UnitPrice.HasValue)
            {
                dettaglioLinea.PrezzoUnitario = item.UnitPrice.Value;
                dettaglioLinea.AliquotaIVA = item.TaxRate?.Rate ?? 0;
            }
            else
            {
                dettaglioLinea.PrezzoUnitario = 0;
                dettaglioLinea.AliquotaIVA = -1; // temp value
            }

            if (item.Quantity.HasValue)
            {
                dettaglioLinea.Quantita = item.Quantity.Value;
                dettaglioLinea.PrezzoTotale = Math.Round(item.UnitPrice.GetValueOrDefault() * item.Quantity.Value, 2, MidpointRounding.AwayFromZero);
            }
            else
                dettaglioLinea.PrezzoTotale = Math.Round(item.UnitPrice.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);

            if (dettaglioLinea.AliquotaIVA == 0)
                dettaglioLinea.Natura = await GetNatureCodeFromIdAsync(item.TaxRate?.TaxRateNatureId);

            body.DatiBeniServizi.DettaglioLinee.Add(dettaglioLinea);
        }

        //Replace temp values in AliquotaIva
        foreach (var linesWithAliquotaIvaTempValue in body.DatiBeniServizi.DettaglioLinee.Where(dl => dl.AliquotaIVA == -1))
        {
            var minAliquotaIvaInPreviousLines = body.DatiBeniServizi.DettaglioLinee.Where(dl => dl.AliquotaIVA > 0).Min(dl => (decimal?)dl.AliquotaIVA);
            if (minAliquotaIvaInPreviousLines.HasValue)
                linesWithAliquotaIvaTempValue.AliquotaIVA = minAliquotaIvaInPreviousLines.Value;
            else
                throw new InvalidOperationException("Non si può generare il file XML se la fattura contiene solo righe descrittive.");
        }

        decimal documentTotalAmount = 0;
        decimal paymentAmount = 0;

        var taxRows = invoice.Rows.GroupBy(r => r.TaxRate?.Rate).Select(g => new
        {
            TaxRate = g.Key,
            NetAmount = Math.Round(g.Sum(r => r.UnitPrice.GetValueOrDefault() * r.Quantity.GetValueOrDefault()), 2, MidpointRounding.AwayFromZero),
            TaxAmount = Math.Round(g.Sum(r => r.UnitPrice.GetValueOrDefault() * r.Quantity.GetValueOrDefault() * (r.TaxRate?.Rate ?? 0) / 100), 2, MidpointRounding.AwayFromZero),
            TaxRateNatureId = g.First().TaxRate?.TaxRateNatureId
        });

        //Dati riepilogo.
        foreach (var item in taxRows)
        {
            if (!item.TaxRate.HasValue)
                continue;

            var datiRiepilogo = new FatturaElettronica.Ordinaria.FatturaElettronicaBody.DatiBeniServizi.DatiRiepilogo()
            {
                AliquotaIVA = item.TaxRate.Value,
                ImponibileImporto = item.NetAmount,
                Imposta = item.TaxAmount
            };

            if (item.TaxRate == 0)
                datiRiepilogo.Natura = await GetNatureCodeFromIdAsync(item.TaxRateNatureId);

            body.DatiBeniServizi.DatiRiepilogo.Add(datiRiepilogo);

            documentTotalAmount += item.NetAmount + item.TaxAmount;
            paymentAmount += item.NetAmount + item.TaxAmount;
        }

        body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento = documentTotalAmount;

        if (invoice.StampDutyAmount.HasValue)
        {
            body.DatiGenerali.DatiGeneraliDocumento.DatiBollo.BolloVirtuale = "SI";
            body.DatiGenerali.DatiGeneraliDocumento.DatiBollo.ImportoBollo = invoice.StampDutyAmount;

            if (invoice.StampDutyChargedToCustomer)
            {
                body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento = documentTotalAmount + invoice.StampDutyAmount;
                paymentAmount += invoice.StampDutyAmount.Value;
            }
        }

        SetDatiPagamento(invoice, paymentAmount, body);

        fattura.FatturaElettronicaBody.Add(body);

        #endregion

        return fattura;
    }

    private void SetCedentePrestatore(FatturaElettronicaHeader header)
    {
        var companyData = appSettings.Value.CompanyData;
        header.CedentePrestatore.DatiAnagrafici.Anagrafica.Denominazione = companyData.Name;
        header.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdPaese = companyData.FatturaElettronicaData?.NazioneSedeCedente;
        header.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdCodice = companyData.VatNumber;
        header.CedentePrestatore.DatiAnagrafici.RegimeFiscale = companyData.FatturaElettronicaData?.RegimeFiscaleCedente;
        header.CedentePrestatore.Sede.Indirizzo = companyData.FatturaElettronicaData?.IndirizzoSedeCedente;
        header.CedentePrestatore.Sede.CAP = companyData.FatturaElettronicaData?.CapSedeCedente;
        header.CedentePrestatore.Sede.Comune = companyData.FatturaElettronicaData?.ComuneSedeCedente;
        header.CedentePrestatore.Sede.Nazione = companyData.FatturaElettronicaData?.NazioneSedeCedente;
    }

    private void SetCessionarioCommittente(FatturaElettronicaHeader header, Customer customer)
    {
        var countryCode = "IT";

        header.CessionarioCommittente.DatiAnagrafici.Anagrafica.Denominazione = customer.Name;
        header.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA.IdPaese = countryCode;
        header.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA.IdCodice = customer.VatNumber;

        header.CessionarioCommittente.Sede.Indirizzo = customer.Addresses.First().Street;
        header.CessionarioCommittente.Sede.CAP = customer.Addresses.First().PostalCode;
        header.CessionarioCommittente.Sede.Comune = customer.Addresses.First().City;
        header.CessionarioCommittente.Sede.Nazione = countryCode;
    }

    private async Task<string?> GetNatureCodeFromIdAsync(int? natureId)
    {
        if (!natureId.HasValue)
            return null;

        TaxRateNature? nature = await taxRateNatureRepository.GetAsync(natureId ?? 0);
        return nature?.Code;
    }

    private void SetDatiPagamento(Invoice invoice, decimal paymentAmount, FatturaElettronicaBody body)
    {
        var dues = invoice.Dues.OrderBy(d => d.Date).ToList();

        var datiPagamento = new FatturaElettronica.Ordinaria.FatturaElettronicaBody.DatiPagamento.DatiPagamento();
        datiPagamento.CondizioniPagamento = dues.Count > 1 ? "TP01" : "TP02";

        // Use the DigitalInvoiceCode from the first payment method found across all dues; fallback to MP05 (rimessa diretta)
        var modalitaPagamento = dues
            .SelectMany(d => d.PaymentDues)
            .Select(pd => pd.Payment?.PaymentMethod?.DigitalInvoiceCode)
            .FirstOrDefault(code => !string.IsNullOrEmpty(code))
            ?? "MP05";

        if (dues.Count > 0)
        {
            foreach (var due in dues)
            {
                var dettaglioPagamento = new FatturaElettronica.Ordinaria.FatturaElettronicaBody.DatiPagamento.DettaglioPagamento();
                dettaglioPagamento.ModalitaPagamento = modalitaPagamento;
                dettaglioPagamento.DataScadenzaPagamento = due.Date.ToDateTime(TimeOnly.MinValue);
                dettaglioPagamento.ImportoPagamento = due.Amount;
                datiPagamento.DettaglioPagamento.Add(dettaglioPagamento);
            }
        }
        else
        {
            var dettaglioPagamento = new FatturaElettronica.Ordinaria.FatturaElettronicaBody.DatiPagamento.DettaglioPagamento();
            dettaglioPagamento.ModalitaPagamento = modalitaPagamento;
            dettaglioPagamento.ImportoPagamento = paymentAmount;
            datiPagamento.DettaglioPagamento.Add(dettaglioPagamento);
        }

        body.DatiPagamento.Add(datiPagamento);
    }

    private Stream CreateXmlStream(FatturaOrdinaria fattura)
    {
        //Convalida del documento.
        ValidationResult result = fattura.Validate();

        //Introspezione errori di convalida.
        StringBuilder errorMessage = new StringBuilder();
        foreach (var error in result.Errors)
        {
            Debug.WriteLine(error.PropertyName);
            Debug.WriteLine(error.ErrorMessage);
            Debug.WriteLine(error.ErrorCode);
            errorMessage.AppendLine($"{error.PropertyName}, {error.ErrorMessage}, ErrorCode: {error.ErrorCode}");
        }

        if (!result.IsValid)
            throw new InvalidOperationException($"Dati non validi per la fattura elettronica. {errorMessage}");

        //Serializzazione XML.
        //using (var w = XmlWriter.Create(xmlFile, new XmlWriterSettings { Indent = true }))
        //{
        //    fattura.WriteXml(w);
        //}

        var stream = new MemoryStream();

        using (var w = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
        {
            fattura.WriteXml(w);
        }

        return stream;
    }
}
