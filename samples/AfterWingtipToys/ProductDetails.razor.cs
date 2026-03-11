using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductDetails
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    private List<Product>? _product;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Product Details";
      using var db = DbFactory.CreateDbContext();
      IQueryable<Product> query = db.Products;

      if (ProductId.HasValue && ProductId > 0)
      {
        query = query.Where(p => p.ProductID == ProductId);
      }
      else if (!string.IsNullOrEmpty(ProductName))
      {
        query = query.Where(p => string.Compare(p.ProductName, ProductName) == 0);
      }
      else
      {
        _product = new List<Product>();
        return;
      }

      _product = await query.ToListAsync();
    }
  }
}
