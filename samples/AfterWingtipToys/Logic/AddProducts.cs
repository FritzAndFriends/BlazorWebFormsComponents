using System;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
  public class AddProducts
  {
    private readonly IDbContextFactory<ProductContext> _dbFactory;

    public AddProducts(IDbContextFactory<ProductContext> dbFactory)
    {
      _dbFactory = dbFactory;
    }

    public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
    {
      var myProduct = new Product
      {
        ProductName = ProductName,
        Description = ProductDesc,
        UnitPrice = Convert.ToDouble(ProductPrice),
        ImagePath = ProductImagePath,
        CategoryID = Convert.ToInt32(ProductCategory)
      };

      using var db = _dbFactory.CreateDbContext();
      db.Products.Add(myProduct);
      db.SaveChanges();
      return true;
    }
  }
}