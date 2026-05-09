using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public static class SeedData
    {
        public static void Initialize(ProductContext context)
        {
            var categories = new[]
            {
                new Category { CategoryID = 1, CategoryName = "Cars", Description = "Cars description" },
                new Category { CategoryID = 2, CategoryName = "Planes", Description = "Planes description" },
                new Category { CategoryID = 3, CategoryName = "Trucks", Description = "Trucks description" },
                new Category { CategoryID = 4, CategoryName = "Boats", Description = "Boats description" },
                new Category { CategoryID = 5, CategoryName = "Rockets", Description = "Rockets description" },
            };
            context.Categories.AddRange(categories);

            var products = new[]
            {
                new Product { ProductName = "Convertible Car", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!", UnitPrice = 22.50, CategoryID = 1, ImagePath = "carconvert.png" },
                new Product { ProductName = "Old-time Car", Description = "There's nothing old about this toy car, except it's looks. It's powered by a neutrino based battery (not included).", UnitPrice = 15.95, CategoryID = 1, ImagePath = "carearly.png" },
                new Product { ProductName = "Fast Car", Description = "Yes this car is fast, but it also floats in water.", UnitPrice = 32.99, CategoryID = 1, ImagePath = "carfast.png" },
                new Product { ProductName = "Super Fast Car", Description = "Use this super fast car to entertain guests. Put in a neutrino battery (not included) and let it go!", UnitPrice = 8.95, CategoryID = 1, ImagePath = "carfaster.png" },
                new Product { ProductName = "Old Style Racer", Description = "This old style racer can fly (with a neutrino battery, not included). Yes, it can fly.", UnitPrice = 34.95, CategoryID = 1, ImagePath = "carracer.png" },
                new Product { ProductName = "Ace Plane", Description = "The Ace Plane is a great flying machine. Fly to the moon in your own toy fighter plane.", UnitPrice = 95.00, CategoryID = 2, ImagePath = "planeace.png" },
                new Product { ProductName = "Move Over Plane", Description = "This plane can move over anything. Even other toy planes.", UnitPrice = 17.00, CategoryID = 2, ImagePath = "planeprop.png" },
                new Product { ProductName = "Paper Plane", Description = "This paper plane flys forever. Well, for a while.", UnitPrice = 8.95, CategoryID = 2, ImagePath = "planepaper.png" },
                new Product { ProductName = "Propeller Plane", Description = "This propeller plane toy takes you around the world. Or at least around your room.", UnitPrice = 32.95, CategoryID = 2, ImagePath = "planeprop.png" },
                new Product { ProductName = "Early Truck", Description = "This toy truck has a real motor! Just plug in a neutrino battery (not included).", UnitPrice = 15.00, CategoryID = 3, ImagePath = "truckearly.png" },
                new Product { ProductName = "Fire Truck", Description = "This fire truck toy is a real fire truck. It even has a siren.", UnitPrice = 26.00, CategoryID = 3, ImagePath = "truckfire.png" },
                new Product { ProductName = "Big Truck", Description = "This big truck can haul anything. Just plug in a neutrino battery (not included).", UnitPrice = 29.00, CategoryID = 3, ImagePath = "truckbig.png" },
                new Product { ProductName = "Big Boat", Description = "This big toy boat can float in your bathtub. It's powered by a neutrino battery (not included).", UnitPrice = 95.00, CategoryID = 4, ImagePath = "boatbig.png" },
                new Product { ProductName = "Sail Boat", Description = "This toy sail boat can float in your bathtub. No batteries needed.", UnitPrice = 9.95, CategoryID = 4, ImagePath = "boatsail.png" },
                new Product { ProductName = "Rocket", Description = "This rocket can fly all the way to the moon. Use a neutrino battery (not included).", UnitPrice = 12.95, CategoryID = 5, ImagePath = "rocket.png" },
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
