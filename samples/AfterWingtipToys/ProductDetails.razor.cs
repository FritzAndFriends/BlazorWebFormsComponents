using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductDetails : ComponentBase
    {
        [Inject]
        private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "id")]
        public int productID { get; set; }

        public Product SampleProduct { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            using var context = DbFactory.CreateDbContext();
            SampleProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductID == productID) ?? new();
        }
    }
}
