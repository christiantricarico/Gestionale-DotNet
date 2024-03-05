using AutoMapper;
using Gdn.Application.TaxRates.Dtos;
using Gdn.Presentation.Shared.Models;

namespace Gdn.Web.Api.Mappings;

public class TaxRateProfile : Profile
{
    public TaxRateProfile()
    {
        CreateMap<TaxRateInputModel, TaxRateInput>();
    }
}
