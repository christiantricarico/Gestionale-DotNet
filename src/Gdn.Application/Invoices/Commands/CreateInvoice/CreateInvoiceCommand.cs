using Gdn.Application.Invoices.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(InvoiceInput Input) : IRequest<Result<Invoice>>;
