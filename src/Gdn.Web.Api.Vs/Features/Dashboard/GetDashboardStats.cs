using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Dashboard;

public class GetDashboardStats
{
    public record GetDashboardStatsResponse(int CustomerCount, int CurrentYearInvoiceCount, int CurrentYearCreditNoteCount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/dashboard", Handler).WithTags(Tags.Dashboard);
        }
    }

    private static async Task<IResult> Handler(ICustomerRepository customerRepository, IInvoiceRepository invoiceRepository, ICreditNoteRepository creditNoteRepository)
    {
        var currentYear = DateTime.UtcNow.Year;

        var customerCount = await customerRepository.CountAsync();
        var invoiceCount = await invoiceRepository.CountAsync(i => i.Date.Year == currentYear);
        var creditNoteCount = await creditNoteRepository.CountAsync(cn => cn.Date.Year == currentYear);

        return ResultHelper.Ok(new GetDashboardStatsResponse(customerCount, invoiceCount, creditNoteCount));
    }
}
