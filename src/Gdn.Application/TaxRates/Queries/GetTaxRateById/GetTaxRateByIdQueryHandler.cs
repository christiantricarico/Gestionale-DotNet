using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Queries.GetTaxRateById;

internal sealed class GetTaxRateByIdQueryHandler : IRequestHandler<GetTaxRateByIdQuery, Result<TaxRate>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository;

    public GetTaxRateByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _taxRateRepository = _unitOfWork.GetRepository<ITaxRateRepository>();
    }

    public async Task<Result<TaxRate>> Handle(GetTaxRateByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _taxRateRepository.GetAsync(request.Id);
        if (data is null)
            return TaxRateErrors.NotFound(request.Id);

        return data;
    }
}
