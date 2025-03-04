using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using MediatR;

namespace Gdn.Application.TaxRates.Commands.DeleteTaxRate;

internal sealed class DeleteTaxRateCommmandHandler : IRequestHandler<DeleteTaxRateCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository;
    private readonly IMapper _mapper;

    public DeleteTaxRateCommmandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _taxRateRepository = _unitOfWork.GetRepository<ITaxRateRepository>();

        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(DeleteTaxRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _taxRateRepository.GetAsync(request.TaxRateId);
        if (entity is null)
            return TaxRateErrors.NotFound(request.TaxRateId);

        entity.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
