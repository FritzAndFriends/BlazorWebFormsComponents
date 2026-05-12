using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductDetails
    {
        [Inject] public ProductContext Db { get; set; } = default!;

        [Parameter] public string? productName { get; set; }
        [SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }

        private FormView<Product> productDetail = default!;
        private Product? ProductItem;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (ProductId.HasValue && ProductId > 0)
            {
                ProductItem = await Db.Products.FirstOrDefaultAsync(p => p.ProductID == ProductId);
            }
            else if (!string.IsNullOrEmpty(productName))
            {
                ProductItem = await Db.Products.FirstOrDefaultAsync(p => p.ProductName == productName);
            }
        }
    }
}
