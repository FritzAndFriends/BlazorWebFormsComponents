using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ProductDetails : WebFormsPageBase
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }
    [Parameter] public string? productName { get; set; }

    private FormView<Product> productDetail = default!;

    protected override async Task OnInitializedAsync()
    {
      await base.OnInitializedAsync();
    }

    public IQueryable<Product> GetProduct(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
      var db = DbFactory.CreateDbContext();
      IQueryable<Product> query = db.Products;
      if (ProductId.HasValue && ProductId > 0)
      {
        query = query.Where(p => p.ProductID == ProductId);
      }
      else if (!string.IsNullOrEmpty(productName))
      {
        query = query.Where(p => p.ProductName == productName);
      }
      else
      {
        totalRowCount = 0;
        return Enumerable.Empty<Product>().AsQueryable();
      }

      var results = query.ToList();
      totalRowCount = results.Count;
      return results.AsQueryable();
    }
  }
}
