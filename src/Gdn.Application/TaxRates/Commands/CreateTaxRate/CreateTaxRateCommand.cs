using Gdn.Application.TaxRates.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Commands.CreateTaxRate;

public sealed record CreateTaxRateCommand(TaxRateInput Input) : IRequest<Result<TaxRate>>;
