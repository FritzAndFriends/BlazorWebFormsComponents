using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductList : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private ProductContext Db { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    private List<Product>? products;

    protected override async Task OnInitializedAsync()
    {
        IQueryable<Product> query = Db.Products;
        if (CategoryId.HasValue && CategoryId > 0)
        {
            query = query.Where(p => p.CategoryID == CategoryId);
        }
        products = await query.ToListAsync();
    }
}
