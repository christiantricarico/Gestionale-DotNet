using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Queries.GetInvoices;

public sealed record GetInvoicesQuery : IRequest<IEnumerable<Invoice>>;
