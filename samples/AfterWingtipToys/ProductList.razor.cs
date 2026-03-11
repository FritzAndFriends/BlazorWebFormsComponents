using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductList
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    [SupplyParameterFromQuery(Name = "categoryName")]
    public string? CategoryName { get; set; }

    private List<Product>? _products;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Products";
      using var db = DbFactory.CreateDbContext();
      IQueryable<Product> query = db.Products.Include(p => p.Category);

      if (CategoryId.HasValue && CategoryId > 0)
      {
        query = query.Where(p => p.CategoryID == CategoryId);
      }

      if (!string.IsNullOrEmpty(CategoryName))
      {
        query = query.Where(p => p.Category != null
                    && string.Compare(p.Category.CategoryName, CategoryName) == 0);
      }

      _products = await query.ToListAsync();
    }
  }
}
