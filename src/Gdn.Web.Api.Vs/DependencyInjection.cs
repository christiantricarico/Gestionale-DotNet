using Gdn.Web.Api.Vs.Features.CreditNotes.Reports;
using Gdn.Web.Api.Vs.Features.CreditNotes.Xml;
using Gdn.Web.Api.Vs.Features.Invoices.Reports;
using Gdn.Web.Api.Vs.Features.Invoices.Xml;
using Gdn.Web.Api.Vs.Features.Payments.Reports;

namespace Gdn.Web.Api.Vs;

public static class DependencyInjection
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddScoped<InvoiceReportGenerator>();
        services.AddScoped<CreditNoteReportGenerator>();
        services.AddScoped<ReceiptReportGenerator>();
        return services;
    }

    public static IServiceCollection AddFatturaElettronica(this IServiceCollection services)
    {
        services.AddScoped<InvoiceXmlGenerator>();
        services.AddScoped<InvoiceXmlFileNameGenerator>();
        services.AddScoped<CreditNoteXmlGenerator>();
        services.AddScoped<CreditNoteXmlFileNameGenerator>();
        return services;
    }
}
