using AutoMapper;
using Gdn.Application.Invoices.Commands.CreateInvoice;
using Gdn.Application.Invoices.Commands.DeleteInvoice;
using Gdn.Application.Invoices.Commands.UpdateInvoice;
using Gdn.Application.Invoices.Dtos;
using Gdn.Application.Invoices.Queries.GetInvoiceById;
using Gdn.Application.Invoices.Queries.GetInvoices;
using Gdn.Presentation.Shared.Models;
using Gdn.Presentation.Shared.Models.Invoices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;

public class InvoiceController : CrudController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public InvoiceController(ISender sender,
                             IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IResult> GetAll()
    {
        var query = new GetInvoicesQuery();
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () =>
            {
                var mappedData = _mapper.Map<IEnumerable<InvoiceViewModel>>(result.Data);
                return Results.Ok(mappedData);
            },
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpGet("{id}")]
    public async Task<IResult> Get(int id)
    {
        var query = new GetInvoiceByIdQuery(id);
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPost("create")]
    public async Task<IResult> Create(InvoiceInputModel model)
    {
        var input = _mapper.Map<InvoiceInput>(model);
        var command = new CreateInvoiceCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Created(GetEntityLocation(result.Data?.Id ?? 0), result.Data),
            onFailure: () => Results.BadRequest(result.Error));
    }

    [HttpPut("update")]
    public async Task<IResult> Update(InvoiceInputModel model)
    {
        var input = _mapper.Map<InvoiceInput>(model);
        var command = new UpdateInvoiceCommand(input);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Data),
            onFailure: () => Results.BadRequest(_mapper.Map<ErrorViewModel>(result.Error)));
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var command = new DeleteInvoiceCommand(id);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: () => Results.BadRequest(result.Error));
    }
}
