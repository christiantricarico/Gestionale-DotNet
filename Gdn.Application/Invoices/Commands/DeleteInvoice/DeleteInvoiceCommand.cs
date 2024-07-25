using MediatR;

namespace Gdn.Application.Invoices.Commands.DeleteInvoice;

public sealed record DeleteInvoiceCommand(int InvoiceId) : IRequest<Result<bool>>;