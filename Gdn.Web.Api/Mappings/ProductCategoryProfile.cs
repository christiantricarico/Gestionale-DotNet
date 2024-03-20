using AutoMapper;
using Gdn.Application.ProductCategories.Dtos;
using Gdn.Domain.Models;
using Gdn.Presentation.Shared.Models.ProductCategories;

namespace Gdn.Web.Api.Mappings;

public class ProductCategoryProfile : Profile
{
    public ProductCategoryProfile()
    {
        CreateMap<ProductCategoryInputModel, ProductCategoryInput>();
        CreateMap<ProductCategory, ProductCategoryViewModel>();
    }
}
