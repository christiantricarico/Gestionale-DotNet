using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Commands.UpdateInvoice;

internal sealed class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Result<Invoice>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public UpdateInvoiceCommandHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.GetRepository<IInvoiceRepository>();
        _mapper = mapper;
    }

    public async Task<Result<Invoice>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        if (!input.Id.HasValue)
            return InvoiceErrors.InvalidInput(nameof(input.Id));

        var entity = await _invoiceRepository.GetAsync(input.Id.Value);
        if (entity is null)
            return InvoiceErrors.NotFound(input.Id.Value);

        _mapper.Map(input, entity);

        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
