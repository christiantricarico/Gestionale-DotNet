using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Queries.GetTaxRateById;

public sealed record GetTaxRateByIdQuery(int Id) : IRequest<Result<TaxRate>>;
