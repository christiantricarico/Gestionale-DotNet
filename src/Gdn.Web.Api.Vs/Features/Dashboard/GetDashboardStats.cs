using System.Security.Claims;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Dashboard;

public class GetDashboardStats
{
    /// <summary>
    /// The counts a coworker is not allowed to see come back as <see langword="null"/>, so the data
    /// never leaves the server rather than merely being hidden by the user interface.
    /// </summary>
    public record GetDashboardStatsResponse(
        int? CustomerCount,
        int? CurrentYearInvoiceCount,
        int? CurrentYearCreditNoteCount,
        int CurrentYearInterventionCount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            // Reachable by any signed in user: the overview page is shared, and the handler decides
            // what each role is allowed to receive.
            app.MapGet("api/dashboard", HandlerAsync).WithTags(Tags.Dashboard);
        }
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal principal,
        ICustomerRepository customerRepository,
        IInvoiceRepository invoiceRepository,
        ICreditNoteRepository creditNoteRepository,
        IInterventionRepository interventionRepository)
    {
        var currentYear = DateTime.UtcNow.Year;

        var interventionCount = await interventionRepository.CountAsync(i => i.Date.Year == currentYear);

        if (!principal.IsInRole(UserRole.Admin))
        {
            return ResultHelper.Ok(new GetDashboardStatsResponse(null, null, null, interventionCount));
        }

        var customerCount = await customerRepository.CountAsync();
        var invoiceCount = await invoiceRepository.CountAsync(i => i.Date.Year == currentYear);
        var creditNoteCount = await creditNoteRepository.CountAsync(cn => cn.Date.Year == currentYear);

        return ResultHelper.Ok(new GetDashboardStatsResponse(
            customerCount,
            invoiceCount,
            creditNoteCount,
            interventionCount));
    }
}
