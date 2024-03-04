using Gdn.Application.TaxRates.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Commands.UpdateTaxRate;

public sealed record UpdateTaxRateCommand(TaxRateInput Input) : IRequest<Result<TaxRate>>;
