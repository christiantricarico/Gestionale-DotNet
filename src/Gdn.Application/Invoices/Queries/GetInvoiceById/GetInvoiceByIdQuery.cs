using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Queries.GetInvoiceById;

public sealed record GetInvoiceByIdQuery(int Id) : IRequest<Result<Invoice>>;
