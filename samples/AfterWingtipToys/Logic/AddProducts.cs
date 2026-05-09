using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class AddProducts(IDbContextFactory<ProductContext> dbFactory)
{
    public bool AddProduct(string productName, string productDesc, string productPrice, string productCategory, string productImagePath)
    {
        var myProduct = new Product
        {
            ProductName = productName,
            Description = productDesc,
            UnitPrice = Convert.ToDouble(productPrice),
            ImagePath = productImagePath,
            CategoryID = Convert.ToInt32(productCategory)
        };

        using var db = dbFactory.CreateDbContext();
        db.Products.Add(myProduct);
        db.SaveChanges();
        return true;
    }
}
