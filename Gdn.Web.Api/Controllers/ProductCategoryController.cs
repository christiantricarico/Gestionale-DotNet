using AutoMapper;
using Gdn.Application.ProductCategories.Commands.CreateProductCategory;
using Gdn.Application.ProductCategories.Commands.DeleteProductCategory;
using Gdn.Application.ProductCategories.Commands.UpdateProductCategory;
using Gdn.Application.ProductCategories.Dtos;
using Gdn.Application.ProductCategories.Queries.GetProductCategories;
using Gdn.Application.ProductCategories.Queries.GetProductCategoryById;
using Gdn.Presentation.Shared.Models;
using Gdn.Presentation.Shared.Models.ProductCategories;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;

[EnableCors("gdn-client-apps")]
public class ProductCategoryController : CrudController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ProductCategoryController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var query = new GetProductCategoriesQuery();
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () =>
            {
                var mappedData = _mapper.Map<IEnumerable<ProductCategoryViewModel>>(result.Data);
                return Results.Ok(mappedData);
            },
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var query = new GetProductCategoryByIdQuery(id);
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPost("create")]
    public async Task<IResult> Create(ProductCategoryInputModel model)
    {
        var input = _mapper.Map<ProductCategoryInput>(model);
        var command = new CreateProductCategoryCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Created(GetEntityLocation(result.Data?.Id ?? 0), result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPut("update")]
    public async Task<IResult> Update(ProductCategoryInputModel model)
    {
        var input = _mapper.Map<ProductCategoryInput>(model);
        var command = new UpdateProductCategoryCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(_mapper.Map<ErrorViewModel>(result.Error)));
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var command = new DeleteProductCatetoryCommand(id);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: () => Results.BadRequest(result.Error));
    }
}
