using AutoMapper;
using Gdn.Application.ProductCategories.Dtos;
using Gdn.Presentation.Shared.Models;

namespace Gdn.Web.Mappings;

public class ProductCategoryProfile : Profile
{
    public ProductCategoryProfile()
    {
        CreateMap<ProductCategoryInputModel, ProductCategoryInput>();
    }
}
