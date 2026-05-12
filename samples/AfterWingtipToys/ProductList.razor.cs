using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductList
    {
        [Inject] public ProductContext Db { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = "id")] public int? CategoryId { get; set; }
        [Parameter] public string? categoryName { get; set; }

        private List<Product> products = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            IQueryable<Product> query = Db.Products.Include(p => p.Category);

            if (CategoryId.HasValue && CategoryId > 0)
            {
                query = query.Where(p => p.CategoryID == CategoryId);
            }
            else if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.CategoryName == categoryName);
            }

            products = await query.ToListAsync();
        }
    }
}
