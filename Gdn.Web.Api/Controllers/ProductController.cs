using AutoMapper;
using Gdn.Application.Products.Commands.CreateProduct;
using Gdn.Application.Products.Commands.DeleteProduct;
using Gdn.Application.Products.Commands.UpdateProduct;
using Gdn.Application.Products.Dtos;
using Gdn.Application.Products.Queries.GetProductById;
using Gdn.Application.Products.Queries.GetProducts;
using Gdn.Presentation.Shared.Models;
using Gdn.Presentation.Shared.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("gdn-client-apps")]
public class ProductController : CrudController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ProductController(ISender sender,
                             IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var query = new GetProductsQuery();
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () =>
            {
                var mappedData = _mapper.Map<IEnumerable<ProductViewModel>>(result.Data);
                return Results.Ok(mappedData);
            },
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPost("create")]
    public async Task<IResult> Create(ProductInputModel model)
    {
        var input = _mapper.Map<ProductInput>(model);
        var command = new CreateProductCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Created(GetEntityLocation(result.Data?.Id ?? 0), result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPut("update")]
    public async Task<IResult> Update(ProductInputModel model)
    {
        var input = _mapper.Map<ProductInput>(model);
        var command = new UpdateProductCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(_mapper.Map<ErrorViewModel>(result.Error)));
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: () => Results.BadRequest(result.Error));
    }
}
