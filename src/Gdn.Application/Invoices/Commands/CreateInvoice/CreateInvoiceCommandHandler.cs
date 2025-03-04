using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Commands.CreateInvoice;

internal sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<Invoice>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.GetRepository<IInvoiceRepository>();
        _mapper = mapper;
    }

    public async Task<Result<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        var entity = _mapper.Map<Invoice>(input);

        _invoiceRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
