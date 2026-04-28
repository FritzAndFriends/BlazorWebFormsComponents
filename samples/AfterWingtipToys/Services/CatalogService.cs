using WingtipToys.Models;

namespace WingtipToys.Services;

public sealed class CatalogService
{
    private readonly IReadOnlyList<Category> _categories;
    private readonly IReadOnlyList<Product> _products;

    public CatalogService()
    {
        _categories =
        [
            new Category { CategoryID = 1, CategoryName = "Boats", Description = "Boats and ships" },
            new Category { CategoryID = 2, CategoryName = "Cars", Description = "Cars and buses" },
            new Category { CategoryID = 3, CategoryName = "Planes", Description = "Planes and rockets" },
            new Category { CategoryID = 4, CategoryName = "Trucks", Description = "Trucks and work vehicles" }
        ];

        _products =
        [
            NewProduct(1, "Paper Boat", "A colorful paper boat for imaginative water adventures.", 1, "boatpaper.png", 8.99),
            NewProduct(2, "Big Boat", "A classic toy boat with bold colors and durable construction.", 1, "boatbig.png", 14.99),
            NewProduct(3, "Sail Boat", "A sail boat inspired by the original Wingtip catalog.", 1, "boatsail.png", 12.49),
            NewProduct(4, "Red Bus", "A bright red bus ready for city routes.", 2, "busred.png", 10.99),
            NewProduct(5, "Fast Car", "A speedy toy car with racing stripes.", 2, "carfast.png", 11.99),
            NewProduct(6, "Racer", "A high-performance toy racer for the fastest laps.", 2, "carracer.png", 13.99),
            NewProduct(7, "Ace Plane", "A stunt-ready prop plane with vivid graphics.", 3, "planeace.png", 15.49),
            NewProduct(8, "Paper Plane", "A playful paper-style plane for flight fans.", 3, "planepaper.png", 7.99),
            NewProduct(9, "Rocket", "A classic rocket toy for out-of-this-world play.", 3, "rocket.png", 16.99),
            NewProduct(10, "Fire Truck", "A fire truck with ladder detail and bright paint.", 4, "truckfire.png", 13.49),
            NewProduct(11, "Big Truck", "A durable truck built for heavy-duty hauling.", 4, "truckbig.png", 14.49)
        ];

        foreach (var product in _products)
        {
            product.Category = _categories.First(category => category.CategoryID == product.CategoryID);
        }
    }

    public IReadOnlyList<Category> GetCategories() => _categories;

    public IReadOnlyList<Product> GetProducts(int? categoryId = null)
    {
        return categoryId.HasValue
            ? _products.Where(product => product.CategoryID == categoryId.Value).ToList()
            : _products;
    }

    public Product? GetProduct(int productId)
    {
        return _products.FirstOrDefault(product => product.ProductID == productId);
    }

    private static Product NewProduct(int id, string name, string description, int categoryId, string imagePath, double price)
    {
        return new Product
        {
            ProductID = id,
            ProductName = name,
            Description = description,
            CategoryID = categoryId,
            ImagePath = imagePath,
            UnitPrice = price
        };
    }
}
