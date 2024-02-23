using Gdn.Application.ProductCategories.Queries.GetProductCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Controllers;

[ApiController]
public class ProductCategoryController : ControllerBase
{
    private readonly ISender _sender;

    public ProductCategoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IResult> GetProductCategory(int id)
    {
        var query = new GetProductCategoryByIdQuery(id);
        var result = await _sender.Send(query);

        //return result.Match(
        //    OnSuccess: data => Results.Ok(data),
        //    OnFailure: error => Results.BadRequest(error));

        if (result.IsSuccess)
            return Results.Ok(result.Data);

        return Results.BadRequest(result.Error);
    }
}
