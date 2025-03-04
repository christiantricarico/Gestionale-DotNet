using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Queries.GetTaxRates;

public sealed record GetTaxRatesQuery : IRequest<Result<IEnumerable<TaxRate>>>;
