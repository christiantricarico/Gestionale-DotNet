using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using MediatR;

namespace Gdn.Application.Invoices.Commands.DeleteInvoice;

internal sealed class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public DeleteInvoiceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.GetRepository<IInvoiceRepository>();
    }

    public async Task<Result<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _invoiceRepository.GetAsync(request.InvoiceId);
        if (entity is null)
            return InvoiceErrors.NotFound(request.InvoiceId);

        entity.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
