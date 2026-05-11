using WingtipToys.Models;

namespace WingtipToys.Logic;

public static class AddProducts
{
    public static bool AddProduct(ProductContext db, string productName, string productDesc, string productPrice, string productCategory, string productImagePath)
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
