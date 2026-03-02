using Microsoft.AspNetCore.Components;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductList : ComponentBase
    {
        public List<Product> Products { get; set; } = new()
        {
            new() { ProductID = 1, ProductName = "Convertible Car", UnitPrice = 22.50m, ImagePath = "car1.png", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!" },
            new() { ProductID = 2, ProductName = "Old-time Car", UnitPrice = 15.95m, ImagePath = "car2.png", Description = "There's nothing old about this toy car, except it's looks. Compatible with other old toy cars." },
            new() { ProductID = 3, ProductName = "Fast Car", UnitPrice = 32.99m, ImagePath = "car3.png", Description = "Yes, this car is fast, but it also floats in water." },
            new() { ProductID = 4, ProductName = "Super Plane", UnitPrice = 8.95m, ImagePath = "plane1.png", Description = "This toy plane can fly. It includes a battery powered engine that allows the plane to fly up to 100 feet." },
        };
    }
}

