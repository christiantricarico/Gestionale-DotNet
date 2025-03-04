using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRateNatures.Queries.GetTaxRateNatures;

internal sealed class GetTaxRateNaturesQueryHandler : IRequestHandler<GetTaxRateNaturesQuery, Result<IEnumerable<TaxRateNature>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateNatureRepository _taxRateNatureRepository;

    public GetTaxRateNaturesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _taxRateNatureRepository = _unitOfWork.GetRepository<ITaxRateNatureRepository>();
    }

    public async Task<Result<IEnumerable<TaxRateNature>>> Handle(GetTaxRateNaturesQuery request, CancellationToken cancellationToken)
    {
        var data = await _taxRateNatureRepository.GetAllAsync();
        return data.ToList();
    }
}
