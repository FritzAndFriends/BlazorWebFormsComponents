using System.Collections.Concurrent;
using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Logic;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseInMemoryDatabase("WingtipToys"));

var registeredUsers = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
    db.Database.EnsureCreated();

    if (!db.Categories.Any())
    {
        db.Categories.AddRange(GetCategories());
        db.Products.AddRange(GetProducts());
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapGet("/Account/PerformLogin", (string? email, string? password, string? returnUrl) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Login?error=Email%20and%20password%20are%20required");
    }

    if (registeredUsers.TryGetValue(email, out var storedPassword) &&
        !string.Equals(storedPassword, password, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Login?error=Invalid%20email%20or%20password");
    }

    var target = $"/Account/Login?loggedIn={Uri.EscapeDataString(email)}";
    if (!string.IsNullOrWhiteSpace(returnUrl))
    {
        target += $"&returnUrl={Uri.EscapeDataString(returnUrl)}";
    }

    return Results.Redirect(target);
});

app.MapGet("/Account/PerformRegister", (string? email, string? password, string? confirmPassword) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Email%20and%20password%20are%20required");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    registeredUsers[email] = password;
    return Results.Redirect("/Account/Login?registered=1");
});

app.MapGet("/ShoppingCart/Update", (int productId, int quantity) =>
{
    using var shoppingCart = new ShoppingCartActions();
    shoppingCart.UpdateItem(shoppingCart.GetCartId(), productId, Math.Max(quantity, 1));
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/ShoppingCart/Remove", (int productId) =>
{
    using var shoppingCart = new ShoppingCartActions();
    shoppingCart.RemoveItem(shoppingCart.GetCartId(), productId);
    return Results.Redirect("/ShoppingCart");
});

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();

static List<Category> GetCategories() =>
[
    new Category { CategoryID = 1, CategoryName = "Cars", Description = "Toy cars" },
    new Category { CategoryID = 2, CategoryName = "Planes", Description = "Toy planes" },
    new Category { CategoryID = 3, CategoryName = "Trucks", Description = "Toy trucks" },
    new Category { CategoryID = 4, CategoryName = "Boats", Description = "Toy boats" },
    new Category { CategoryID = 5, CategoryName = "Rockets", Description = "Toy rockets" }
];

static List<Product> GetProducts() =>
[
    new Product
    {
        ProductID = 1,
        ProductName = "Convertible Car",
        Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included).Power it up and let it go!",
        ImagePath = "carconvert.png",
        UnitPrice = 22.50,
        CategoryID = 1
    },
    new Product
    {
        ProductID = 2,
        ProductName = "Old-time Car",
        Description = "There's nothing old about this toy car, except it's looks. Compatible with other old toy cars.",
        ImagePath = "carearly.png",
        UnitPrice = 15.95,
        CategoryID = 1
    },
    new Product
    {
        ProductID = 3,
        ProductName = "Fast Car",
        Description = "Yes this car is fast, but it also floats in water.",
        ImagePath = "carfast.png",
        UnitPrice = 32.99,
        CategoryID = 1
    },
    new Product
    {
        ProductID = 4,
        ProductName = "Super Fast Car",
        Description = "Use this super fast car to entertain guests. Lights and doors work!",
        ImagePath = "carfaster.png",
        UnitPrice = 8.95,
        CategoryID = 1
    },
    new Product
    {
        ProductID = 5,
        ProductName = "Old Style Racer",
        Description = "This old style racer can fly (with user assistance). Gravity controls flight duration.No batteries required.",
        ImagePath = "carracer.png",
        UnitPrice = 34.95,
        CategoryID = 1
    },
    new Product
    {
        ProductID = 6,
        ProductName = "Ace Plane",
        Description = "Authentic airplane toy. Features realistic color and details.",
        ImagePath = "planeace.png",
        UnitPrice = 95.00,
        CategoryID = 2
    },
    new Product
    {
        ProductID = 7,
        ProductName = "Glider",
        Description = "This fun glider is made from real balsa wood. Some assembly required.",
        ImagePath = "planeglider.png",
        UnitPrice = 4.95,
        CategoryID = 2
    },
    new Product
    {
        ProductID = 8,
        ProductName = "Paper Plane",
        Description = "This paper plane is like no other paper plane. Some folding required.",
        ImagePath = "planepaper.png",
        UnitPrice = 2.95,
        CategoryID = 2
    },
    new Product
    {
        ProductID = 9,
        ProductName = "Propeller Plane",
        Description = "Rubber band powered plane features two wheels.",
        ImagePath = "planeprop.png",
        UnitPrice = 32.95,
        CategoryID = 2
    },
    new Product
    {
        ProductID = 10,
        ProductName = "Early Truck",
        Description = "This toy truck has a real gas powered engine. Requires regular tune ups.",
        ImagePath = "truckearly.png",
        UnitPrice = 15.00,
        CategoryID = 3
    },
    new Product
    {
        ProductID = 11,
        ProductName = "Fire Truck",
        Description = "You will have endless fun with this one quarter sized fire truck.",
        ImagePath = "truckfire.png",
        UnitPrice = 26.00,
        CategoryID = 3
    },
    new Product
    {
        ProductID = 12,
        ProductName = "Big Truck",
        Description = "This fun toy truck can be used to tow other trucks that are not as big.",
        ImagePath = "truckbig.png",
        UnitPrice = 29.00,
        CategoryID = 3
    },
    new Product
    {
        ProductID = 13,
        ProductName = "Big Ship",
        Description = "Is it a boat or a ship. Let this floating vehicle decide by using its artifically intelligent computer brain!",
        ImagePath = "boatbig.png",
        UnitPrice = 95.00,
        CategoryID = 4
    },
    new Product
    {
        ProductID = 14,
        ProductName = "Paper Boat",
        Description = "Floating fun for all! This toy boat can be assembled in seconds. Floats for minutes!Some folding required.",
        ImagePath = "boatpaper.png",
        UnitPrice = 4.95,
        CategoryID = 4
    },
    new Product
    {
        ProductID = 15,
        ProductName = "Sail Boat",
        Description = "Put this fun toy sail boat in the water and let it go!",
        ImagePath = "boatsail.png",
        UnitPrice = 42.95,
        CategoryID = 4
    },
    new Product
    {
        ProductID = 16,
        ProductName = "Rocket",
        Description = "This fun rocket will travel up to a height of 200 feet.",
        ImagePath = "rocket.png",
        UnitPrice = 122.95,
        CategoryID = 5
    }
];
