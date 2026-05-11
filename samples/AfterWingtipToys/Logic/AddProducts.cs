using System;
using WingtipToys.Models;

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
      var myProduct = new Product
      {
        ProductName = ProductName,
        Description = ProductDesc,
        UnitPrice = Convert.ToDouble(ProductPrice),
        ImagePath = ProductImagePath,
        CategoryID = Convert.ToInt32(ProductCategory)
      };

      _db.Products.Add(myProduct);
      _db.SaveChanges();
      return true;
    }
  }
}