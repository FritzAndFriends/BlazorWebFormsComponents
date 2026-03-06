using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private ProductContext Db { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductID { get; set; }

    private Product? product;

    protected override async Task OnInitializedAsync()
    {
        if (ProductID.HasValue && ProductID > 0)
        {
            product = await Db.Products.FirstOrDefaultAsync(p => p.ProductID == ProductID);
        }
    }
}
