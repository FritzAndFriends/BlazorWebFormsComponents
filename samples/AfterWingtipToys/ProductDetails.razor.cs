using System.Linq;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductDetails
  {
    [Inject] public ProductContext Db { get; set; } = default!;
    [Parameter, SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }
    [Parameter] public string? ProductName { get; set; }

    public IQueryable<Product> GetProduct(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
      IQueryable<Product> query = Db.Products;
      if (ProductId.HasValue && ProductId > 0)
      {
        query = query.Where(p => p.ProductID == ProductId.Value);
      }
      else if (!string.IsNullOrEmpty(ProductName))
      {
        query = query.Where(p => p.ProductName == ProductName);
      }
      else
      {
        query = Enumerable.Empty<Product>().AsQueryable();
      }

      totalRowCount = query.Count();
      return query;
    }
  }
}