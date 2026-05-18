using System.Collections.Generic;
using System.Linq;

namespace WingtipToys.Models;

/// <summary>
/// Stub for quarantined class. Provides compile compatibility for dependent code.
/// Replace with a proper implementation during Layer 2 migration.
/// </summary>
public class ProductDatabaseInitializer
{
    public static void SeedIfNeeded(ProductContext context)
    {
        if (context.Categories.Any() || context.Products.Any())
        {
            return;
        }

        var categories = GetCategories();
        context.Categories.AddRange(categories);
        context.SaveChanges();

        var categoryLookup = context.Categories.ToDictionary(category => category.CategoryName, category => category.CategoryID);
        var products = GetProducts(categoryLookup);
        context.Products.AddRange(products);
        context.SaveChanges();
    }

    private static List<Category> GetCategories() =>
    [
        new Category { CategoryName = "Cars", Description = "Toy cars" },
        new Category { CategoryName = "Planes", Description = "Toy planes" },
        new Category { CategoryName = "Trucks", Description = "Toy trucks" },
        new Category { CategoryName = "Boats", Description = "Toy boats" },
        new Category { CategoryName = "Rockets", Description = "Toy rockets" }
    ];

    private static List<Product> GetProducts(IReadOnlyDictionary<string, int> categories) =>
    [
        new Product { ProductName = "Convertible Car", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!", ImagePath = "carconvert.png", UnitPrice = 22.50, CategoryID = categories["Cars"] },
        new Product { ProductName = "Old-time Car", Description = "There's nothing old about this toy car, except its looks. Compatible with other old toy cars.", ImagePath = "carearly.png", UnitPrice = 15.95, CategoryID = categories["Cars"] },
        new Product { ProductName = "Fast Car", Description = "Yes this car is fast, but it also floats in water.", ImagePath = "carfast.png", UnitPrice = 32.99, CategoryID = categories["Cars"] },
        new Product { ProductName = "Super Fast Car", Description = "Use this super fast car to entertain guests. Lights and doors work!", ImagePath = "carfaster.png", UnitPrice = 8.95, CategoryID = categories["Cars"] },
        new Product { ProductName = "Old Style Racer", Description = "This old style racer can fly (with user assistance). Gravity controls flight duration. No batteries required.", ImagePath = "carracer.png", UnitPrice = 34.95, CategoryID = categories["Cars"] },
        new Product { ProductName = "Ace Plane", Description = "Authentic airplane toy. Features realistic color and details.", ImagePath = "planeace.png", UnitPrice = 95.00, CategoryID = categories["Planes"] },
        new Product { ProductName = "Glider", Description = "This fun glider is made from real balsa wood. Some assembly required.", ImagePath = "planeglider.png", UnitPrice = 4.95, CategoryID = categories["Planes"] },
        new Product { ProductName = "Paper Plane", Description = "This paper plane is like no other paper plane. Some folding required.", ImagePath = "planepaper.png", UnitPrice = 2.95, CategoryID = categories["Planes"] },
        new Product { ProductName = "Propeller Plane", Description = "Rubber band powered plane features two wheels.", ImagePath = "planeprop.png", UnitPrice = 32.95, CategoryID = categories["Planes"] },
        new Product { ProductName = "Early Truck", Description = "This toy truck has a real gas powered engine. Requires regular tune ups.", ImagePath = "truckearly.png", UnitPrice = 15.00, CategoryID = categories["Trucks"] },
        new Product { ProductName = "Fire Truck", Description = "You will have endless fun with this one quarter sized fire truck.", ImagePath = "truckfire.png", UnitPrice = 26.00, CategoryID = categories["Trucks"] },
        new Product { ProductName = "Big Truck", Description = "This fun toy truck can be used to tow other trucks that are not as big.", ImagePath = "truckbig.png", UnitPrice = 29.00, CategoryID = categories["Trucks"] },
        new Product { ProductName = "Big Ship", Description = "Is it a boat or a ship? Let this floating vehicle decide by using its artificially intelligent computer brain!", ImagePath = "boatbig.png", UnitPrice = 95.00, CategoryID = categories["Boats"] },
        new Product { ProductName = "Paper Boat", Description = "Floating fun for all! This toy boat can be assembled in seconds. Floats for minutes! Some folding required.", ImagePath = "boatpaper.png", UnitPrice = 4.95, CategoryID = categories["Boats"] },
        new Product { ProductName = "Sail Boat", Description = "Put this fun toy sail boat in the water and let it go!", ImagePath = "boatsail.png", UnitPrice = 42.95, CategoryID = categories["Boats"] },
        new Product { ProductName = "Rocket", Description = "This fun rocket will travel up to a height of 200 feet.", ImagePath = "rocket.png", UnitPrice = 122.95, CategoryID = categories["Rockets"] }
    ];
}
