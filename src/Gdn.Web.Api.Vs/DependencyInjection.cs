using Gdn.Web.Api.Vs.Features.Invoices.Reports;
using Gdn.Web.Api.Vs.Features.Invoices.Xml;

namespace Gdn.Web.Api.Vs;

public static class DependencyInjection
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddScoped<InvoiceReportGenerator>();
        return services;
    }

    public static IServiceCollection AddFatturaElettronica(this IServiceCollection services)
    {
        services.AddScoped<InvoiceXmlGenerator>();
        services.AddScoped<InvoiceXmlFileNameGenerator>();
        return services;
    }
}
