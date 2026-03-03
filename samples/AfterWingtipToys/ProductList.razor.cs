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
            new() { ProductID = 4, ProductName = "Super Fast Car", UnitPrice = 8.95m, ImagePath = "carfaster.png", CategoryID = 1 },
            new() { ProductID = 5, ProductName = "Old Style Racer", UnitPrice = 34.95m, ImagePath = "carracer.png", CategoryID = 1 },
            new() { ProductID = 6, ProductName = "Ace Plane", UnitPrice = 95.00m, ImagePath = "planeace.png", CategoryID = 2 },
            new() { ProductID = 7, ProductName = "Glider", UnitPrice = 4.95m, ImagePath = "planeglider.png", CategoryID = 2 },
            new() { ProductID = 8, ProductName = "Paper Plane", UnitPrice = 2.95m, ImagePath = "planepaper.png", CategoryID = 2 },
            new() { ProductID = 9, ProductName = "Propeller Plane", UnitPrice = 32.95m, ImagePath = "planeprop.png", CategoryID = 2 },
            new() { ProductID = 10, ProductName = "Early Truck", UnitPrice = 15.00m, ImagePath = "truckearly.png", CategoryID = 3 },
            new() { ProductID = 11, ProductName = "Fire Truck", UnitPrice = 26.00m, ImagePath = "truckfire.png", CategoryID = 3 },
            new() { ProductID = 12, ProductName = "Big Truck", UnitPrice = 29.00m, ImagePath = "truckbig.png", CategoryID = 3 },
            new() { ProductID = 13, ProductName = "Big Ship", UnitPrice = 95.00m, ImagePath = "boatbig.png", CategoryID = 4 },
            new() { ProductID = 14, ProductName = "Paper Boat", UnitPrice = 4.95m, ImagePath = "boatpaper.png", CategoryID = 4 },
            new() { ProductID = 15, ProductName = "Sail Boat", UnitPrice = 42.95m, ImagePath = "boatsail.png", CategoryID = 4 },
            new() { ProductID = 16, ProductName = "Rocket", UnitPrice = 122.95m, ImagePath = "rocket.png", CategoryID = 5 },
        };
    }
}

