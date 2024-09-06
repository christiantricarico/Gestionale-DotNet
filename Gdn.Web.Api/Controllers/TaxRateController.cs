using AutoMapper;
using Gdn.Application.TaxRates.Commands.CreateTaxRate;
using Gdn.Application.TaxRates.Commands.DeleteTaxRate;
using Gdn.Application.TaxRates.Commands.UpdateTaxRate;
using Gdn.Application.TaxRates.Dtos;
using Gdn.Application.TaxRates.Queries.GetTaxRateById;
using Gdn.Application.TaxRates.Queries.GetTaxRates;
using Gdn.Presentation.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;

public class TaxRateController : CrudController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public TaxRateController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var query = new GetTaxRatesQuery();
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var query = new GetTaxRateByIdQuery(id);
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPost("create")]
    public async Task<IResult> Create(TaxRateInputModel model)
    {
        var input = _mapper.Map<TaxRateInput>(model);
        var command = new CreateTaxRateCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Created(GetEntityLocation(result.Data?.Id ?? 0), result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPut("update")]
    public async Task<IResult> Update(TaxRateInputModel model)
    {
        var input = _mapper.Map<TaxRateInput>(model);
        var command = new UpdateTaxRateCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var command = new DeleteTaxRateCommand(id);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: () => Results.BadRequest(result.Error));
    }
}
