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
        [Parameter, SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }

        private FormView<Product> productDetail = default!;
        private Product? ProductItem { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            ProductItem = await GetProduct().FirstOrDefaultAsync();
        }

        public IQueryable<Product> GetProduct()
        {
            IQueryable<Product> query = Db.Products;
            if (ProductId.HasValue && ProductId > 0)
            {
                query = query.Where(p => p.ProductID == ProductId.Value);
            }
            else if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => p.ProductName == productName);
            }
            else
            {
                return Enumerable.Empty<Product>().AsQueryable();
            }

            return query;
        }
    }
}
