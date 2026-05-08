using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject]
    private ProductContext ProductContext { get; set; } = default!;

    [Parameter]
    public int ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? QueryProductId { get; set; }

    [Parameter]
    public string? ProductName { get; set; }

    private Product? SelectedProduct { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var resolvedId = ProductId > 0 ? ProductId : QueryProductId.GetValueOrDefault();
        if (resolvedId > 0)
        {
            SelectedProduct = await ProductContext.Products.FirstOrDefaultAsync(p => p.ProductID == resolvedId);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ProductName))
        {
            SelectedProduct = await ProductContext.Products.FirstOrDefaultAsync(p => p.ProductName == ProductName);
            return;
        }

        SelectedProduct = null;
    }
}
