using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Queries.GetTaxRates;

internal sealed class GetTaxRatesQueryHandler : IRequestHandler<GetTaxRatesQuery, Result<IEnumerable<TaxRate>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository;

    public GetTaxRatesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _taxRateRepository = _unitOfWork.GetRepository<ITaxRateRepository>();
    }

    public async Task<Result<IEnumerable<TaxRate>>> Handle(GetTaxRatesQuery request, CancellationToken cancellationToken)
    {
        var data = await _taxRateRepository.GetAllAsync();
        return data.ToList();
    }
}
