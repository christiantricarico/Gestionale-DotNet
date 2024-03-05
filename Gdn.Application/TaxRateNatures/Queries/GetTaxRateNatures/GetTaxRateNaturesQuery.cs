using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRateNatures.Queries.GetTaxRateNatures;

public sealed record GetTaxRateNaturesQuery : IRequest<Result<IEnumerable<TaxRateNature>>>;
