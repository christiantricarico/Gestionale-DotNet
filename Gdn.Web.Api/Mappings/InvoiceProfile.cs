using AutoMapper;
using Gdn.Application.Invoices.Dtos;
using Gdn.Domain.Models;
using Gdn.Presentation.Shared.Models.Invoices;

namespace Gdn.Web.Api.Mappings;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<InvoiceInputModel, InvoiceInput>();
        CreateMap<Invoice, InvoiceViewModel>();
    }
}
