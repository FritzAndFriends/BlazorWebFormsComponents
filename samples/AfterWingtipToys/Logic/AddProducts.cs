using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;
using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Logic
{
  public class AddProducts
  {
    private readonly ProductContext _db;

    public AddProducts(ProductContext db)
    {
      _db = db;
    }

    public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
    {
      var myProduct = new Product();
      myProduct.ProductName = ProductName;
      myProduct.Description = ProductDesc;
      myProduct.UnitPrice = Convert.ToDouble(ProductPrice);
      myProduct.ImagePath = ProductImagePath;
      myProduct.CategoryID = Convert.ToInt32(ProductCategory);

      _db.Products.Add(myProduct);
      _db.SaveChanges();

      return true;
    }
  }
}