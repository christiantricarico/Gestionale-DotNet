using Gdn.Domain.Data.Repositories;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.Payments.Reports;

/// <summary>
/// Generates the PDF receipt document (distinta di incasso) for a registered payment.
/// </summary>
public class ReceiptReportGenerator(IOptions<AppSettings> appSettings, IPaymentRepository paymentRepository)
{
    /// <summary>
    /// Generates PDF bytes for the receipt identified by <paramref name="paymentId"/>.
    /// </summary>
    /// <param name="paymentId">The ID of the payment (incasso) to generate the PDF for.</param>
    /// <returns>Raw PDF bytes.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the payment is not found.</exception>
    public async Task<byte[]> GeneratePdfBytesAsync(int paymentId)
    {
        var model = await GetReportDataAsync(paymentId);
        var document = new ReceiptDocument(model);
        return document.GeneratePdf();
    }

    private async Task<ReceiptReportModel> GetReportDataAsync(int paymentId)
    {
        var payment = await paymentRepository.GetAsync(paymentId,
            ["PaymentMethod", "Customer.Addresses", "PaymentDues.Due.Invoice"])
            ?? throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

        var company = appSettings.Value.CompanyData;
        var customer = payment.Customer;
        var customerAddress = customer?.Addresses.FirstOrDefault();

        return new ReceiptReportModel
        {
            PaymentId = payment.Id,
            Date = payment.Date,
            Amount = payment.Amount,
            PaymentMethodName = payment.PaymentMethod?.Name,
            SellerAddress = new AddressModel
            {
                CompanyName = company.Name,
                Street = company.Street,
                PostalCode = company.PostalCode,
                City = company.City,
                Province = company.Province,
                Email = company.Email,
                Phone = company.Phone
            },
            CustomerAddress = new AddressModel
            {
                CompanyName = customer?.Name,
                Street = customerAddress?.Street,
                PostalCode = customerAddress?.PostalCode,
                City = customerAddress?.City,
                Province = customerAddress?.Province,
                Email = customer?.Email,
                Phone = customer?.Phone
            },
            CoveredDues = payment.PaymentDues
                .OrderBy(pd => pd.Due.Date)
                .Select(pd => new ReceiptDueRowModel
                {
                    DueDate = pd.Due.Date,
                    DocumentNumber = pd.Due.Invoice?.Number,
                    DocumentType = pd.Due.Invoice is not null ? "Fattura" : "Scadenza",
                    DueAmount = pd.Due.Amount,
                    AllocatedAmount = pd.Amount
                }).ToList()
        };
    }
}
