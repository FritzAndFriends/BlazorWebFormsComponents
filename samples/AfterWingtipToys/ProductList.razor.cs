using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ProductList : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public ProductCatalogService Catalog { get; set; } = default!;
    [Parameter, SupplyParameterFromQuery(Name = "id")] public int? CategoryId { get; set; }
    [Parameter] public string? CategoryName { get; set; }

    protected IReadOnlyList<Product> Products { get; private set; } = [];

    protected override void OnParametersSet()
    {
        Products = Catalog.GetProducts(CategoryId, CategoryName);
        Title = string.IsNullOrWhiteSpace(CategoryName) ? "Products" : CategoryName;
        base.OnParametersSet();
    }
}