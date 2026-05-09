using Microsoft.AspNetCore.Components;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject]
    public CatalogService CatalogService { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productId")]
    public int? AlternateProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    protected Product? Product { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Product = await CatalogService.GetProductAsync(ProductId ?? AlternateProductId, ProductName);
    }
}
