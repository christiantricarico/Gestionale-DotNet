using Gdn.Application.TaxRateNatures.Queries.GetTaxRateNatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxRateNatureController : ControllerBase
{
    private readonly ISender _sender;

    public TaxRateNatureController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var query = new GetTaxRateNaturesQuery();
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }
}
