using Gdn.Application.Invoices.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Commands.UpdateInvoice;

public sealed record UpdateInvoiceCommand(InvoiceInput Input) : IRequest<Result<Invoice>>;
