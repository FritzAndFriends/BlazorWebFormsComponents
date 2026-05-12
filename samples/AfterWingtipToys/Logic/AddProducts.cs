using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
  public class AddProducts
  {
    public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
    {
      var myProduct = new Product();
      myProduct.ProductName = ProductName;
      myProduct.Description = ProductDesc;
      myProduct.UnitPrice = Convert.ToDouble(ProductPrice);
      myProduct.ImagePath = ProductImagePath;
      myProduct.CategoryID = Convert.ToInt32(ProductCategory);

      var connectionString = BlazorWebFormsComponents.ConfigurationManager.ConnectionStrings["WingtipToys"]?.ConnectionString
          ?? throw new InvalidOperationException("Connection string 'WingtipToys' was not found.");
      var options = new DbContextOptionsBuilder<ProductContext>()
          .UseSqlServer(connectionString)
          .Options;

      using (ProductContext _db = new ProductContext(options))
      {
        _db.Products.Add(myProduct);
        _db.SaveChanges();
      }

      return true;
    }
  }
}
