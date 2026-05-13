using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;

using Microsoft.AspNetCore.Components;
namespace WingtipToys.Logic
{
  public class AddProducts
  {
    [Inject]
    protected ProductContext _productContext { get; set; } = default!;

    public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
    {
      var myProduct = new Product();
      myProduct.ProductName = ProductName;
      myProduct.Description = ProductDesc;
      myProduct.UnitPrice = Convert.ToDouble(ProductPrice);
      myProduct.ImagePath = ProductImagePath;
      myProduct.CategoryID = Convert.ToInt32(ProductCategory);

      // DbContext 'ProductContext' is injected via DI

        // Add product to DB.
        _productContext.Products.Add(myProduct);
        _productContext.SaveChanges();
      
      // Success.
      return true;
    }
  }
}