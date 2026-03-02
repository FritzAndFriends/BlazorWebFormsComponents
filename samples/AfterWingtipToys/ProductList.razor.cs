using Microsoft.AspNetCore.Components;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductList : ComponentBase
    {
        public List<Product> Products { get; set; } = new()
        {
            new() { ProductID = 1, ProductName = "Convertible Car", UnitPrice = 22.50m, ImagePath = "carconvert.png", CategoryID = 1 },
            new() { ProductID = 2, ProductName = "Old-time Car", UnitPrice = 15.95m, ImagePath = "carearly.png", CategoryID = 1 },
            new() { ProductID = 3, ProductName = "Fast Car", UnitPrice = 32.99m, ImagePath = "carfast.png", CategoryID = 1 },
            new() { ProductID = 4, ProductName = "Super Plane", UnitPrice = 8.95m, ImagePath = "planeprop.png", CategoryID = 2 },
            new() { ProductID = 5, ProductName = "Paper Plane", UnitPrice = 4.95m, ImagePath = "planepaper.png", CategoryID = 2 },
            new() { ProductID = 6, ProductName = "Sail Boat", UnitPrice = 14.95m, ImagePath = "boatsail.png", CategoryID = 3 },
            new() { ProductID = 7, ProductName = "Rocket", UnitPrice = 12.95m, ImagePath = "rocket.png", CategoryID = 4 },
            new() { ProductID = 8, ProductName = "Fire Truck", UnitPrice = 26.00m, ImagePath = "truckfire.png", CategoryID = 1 },
        };
    }
}

