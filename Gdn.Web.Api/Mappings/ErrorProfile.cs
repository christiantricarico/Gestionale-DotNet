using AutoMapper;
using Gdn.Application;
using Gdn.Presentation.Shared.Models;

namespace Gdn.Web.Api.Mappings;

public class ErrorProfile : Profile
{
    public ErrorProfile()
    {
        CreateMap<Error, ErrorViewModel>();
    }
}
