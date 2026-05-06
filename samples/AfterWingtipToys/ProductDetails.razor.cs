using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductDetails
  {
    [Inject] private ProductContext Db { get; set; } = default!;

    private FormView<Product> productDetail = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    protected override async Task OnInitializedAsync()
    {
      await base.OnInitializedAsync();
    }

    public IEnumerable<Product> GetProduct()
    {
      var query = Db.Products.Include(p => p.Category).AsQueryable();

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
        return Enumerable.Empty<Product>();
      }

      return query.ToList();
    }
  }
}
