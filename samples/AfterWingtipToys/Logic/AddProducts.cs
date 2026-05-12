using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class AddProducts
{
    private const string WingtipToysConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=WingtipToys;Integrated Security=True";

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

        var options = new DbContextOptionsBuilder<ProductContext>()
            .UseSqlServer(WingtipToysConnectionString)
            .Options;

        using var db = new ProductContext(options);
        db.Products.Add(myProduct);
        db.SaveChanges();
        return true;
    }
}
