using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductList
  {
    [Inject] private ProductContext Db { get; set; } = default!;

    private ListView<Product> productList = default!;

    [Parameter, SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    [Parameter]
    public string? CategoryName { get; set; }

    protected IReadOnlyList<Product> Products { get; private set; } = [];

    protected override async Task OnParametersSetAsync()
    {
      await base.OnParametersSetAsync();

      IQueryable<Product> query = Db.Products
        .Include(p => p.Category)
        .OrderBy(p => p.ProductID);

      if (CategoryId.HasValue && CategoryId.Value > 0)
      {
        query = query.Where(p => p.CategoryID == CategoryId.Value);
      }

      if (!string.IsNullOrWhiteSpace(CategoryName))
      {
        query = query.Where(p => p.Category != null && p.Category.CategoryName == CategoryName);
      }

      Products = await query.ToListAsync();
    }
  }
}
