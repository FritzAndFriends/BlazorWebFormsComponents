using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductDetails
  {
    [Inject] private ProductContext Db { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [Parameter]
    public string? ProductName { get; set; }

    protected Product? Product { get; private set; }

    protected override async Task OnParametersSetAsync()
    {
      await base.OnParametersSetAsync();

      IQueryable<Product> query = Db.Products.AsNoTracking();

      if (ProductId.HasValue && ProductId.Value > 0)
      {
        query = query.Where(p => p.ProductID == ProductId.Value);
      }
      else if (!string.IsNullOrWhiteSpace(ProductName))
      {
        query = query.Where(p => p.ProductName == ProductName);
      }
      else
      {
        Product = null;
        return;
      }

      Product = await query.FirstOrDefaultAsync();
    }
  }
}
