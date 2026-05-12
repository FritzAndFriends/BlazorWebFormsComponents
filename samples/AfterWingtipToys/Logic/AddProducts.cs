using WingtipToys.Models;

namespace WingtipToys.Logic;

public class AddProducts(ProductContext db)
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

        db.Products.Add(myProduct);
        db.SaveChanges();
        return true;
    }
}