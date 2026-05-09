using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.ProductCategories;

public class ProductCategoryViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Livello")]
    public int Level { get; set; }

    public int? ParentCategoryId { get; set; }

    [Display(Name = "Categoria padre")]
    public string? ParentCategoryName { get; set; }
}
