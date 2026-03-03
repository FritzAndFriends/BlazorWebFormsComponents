using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductList : ComponentBase
    {
        [Inject]
        private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

        [SupplyParameterFromQuery]
        public int? id { get; set; }

        public List<Product> Products { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            using var context = DbFactory.CreateDbContext();
            var query = context.Products.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(p => p.CategoryID == id.Value);
            }

            Products = await query.ToListAsync();
        }
    }
}

