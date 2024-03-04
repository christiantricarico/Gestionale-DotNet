using MediatR;

namespace Gdn.Application.TaxRates.Commands.DeleteTaxRate;

public sealed record DeleteTaxRateCommand(int TaxRateId) : IRequest<Result<bool>>;
