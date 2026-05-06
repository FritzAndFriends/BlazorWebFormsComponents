using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=WingtipToysApp;Trusted_Connection=True;";
var wingtipConnection = builder.Configuration.GetConnectionString("WingtipToys")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=WingtipToys;Trusted_Connection=True;";

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ProductContext>(options => options.UseSqlServer(wingtipConnection));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(defaultConnection));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<WingtipToys.Logic.CartSessionStore>();
builder.Services.AddScoped<WingtipToys.Logic.CartService>();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.UseConfigurationManagerShim();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var productDb = services.GetRequiredService<ProductContext>();
    var identityDb = services.GetRequiredService<ApplicationDbContext>();

    productDb.Database.EnsureCreated();
    identityDb.Database.EnsureCreated();

    if (!productDb.Categories.Any())
    {
        var categories = new List<Category>
        {
            new() { CategoryID = 1, CategoryName = "Cars" },
            new() { CategoryID = 2, CategoryName = "Planes" },
            new() { CategoryID = 3, CategoryName = "Trucks" },
            new() { CategoryID = 4, CategoryName = "Boats" },
            new() { CategoryID = 5, CategoryName = "Rockets" }
        };

        var products = new List<Product>
        {
            new() { ProductID = 1, ProductName = "Convertible Car", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included).Power it up and let it go!", ImagePath = "carconvert.png", UnitPrice = 22.50, CategoryID = 1 },
            new() { ProductID = 2, ProductName = "Old-time Car", Description = "There's nothing old about this toy car, except it's looks. Compatible with other old toy cars.", ImagePath = "carearly.png", UnitPrice = 15.95, CategoryID = 1 },
            new() { ProductID = 3, ProductName = "Fast Car", Description = "Yes this car is fast, but it also floats in water.", ImagePath = "carfast.png", UnitPrice = 32.99, CategoryID = 1 },
            new() { ProductID = 4, ProductName = "Super Fast Car", Description = "Use this super fast car to entertain guests. Lights and doors work!", ImagePath = "carfaster.png", UnitPrice = 8.95, CategoryID = 1 },
            new() { ProductID = 5, ProductName = "Old Style Racer", Description = "This old style racer can fly (with user assistance). Gravity controls flight duration.No batteries required.", ImagePath = "carracer.png", UnitPrice = 34.95, CategoryID = 1 },
            new() { ProductID = 6, ProductName = "Ace Plane", Description = "Authentic airplane toy. Features realistic color and details.", ImagePath = "planeace.png", UnitPrice = 95.00, CategoryID = 2 },
            new() { ProductID = 7, ProductName = "Glider", Description = "This fun glider is made from real balsa wood. Some assembly required.", ImagePath = "planeglider.png", UnitPrice = 4.95, CategoryID = 2 },
            new() { ProductID = 8, ProductName = "Paper Plane", Description = "This paper plane is like no other paper plane. Some folding required.", ImagePath = "planepaper.png", UnitPrice = 2.95, CategoryID = 2 },
            new() { ProductID = 9, ProductName = "Propeller Plane", Description = "Rubber band powered plane features two wheels.", ImagePath = "planeprop.png", UnitPrice = 32.95, CategoryID = 2 },
            new() { ProductID = 10, ProductName = "Early Truck", Description = "This toy truck has a real gas powered engine. Requires regular tune ups.", ImagePath = "truckearly.png", UnitPrice = 15.00, CategoryID = 3 },
            new() { ProductID = 11, ProductName = "Fire Truck", Description = "You will have endless fun with this one quarter sized fire truck.", ImagePath = "truckfire.png", UnitPrice = 26.00, CategoryID = 3 },
            new() { ProductID = 12, ProductName = "Big Truck", Description = "This fun toy truck can be used to tow other trucks that are not as big.", ImagePath = "truckbig.png", UnitPrice = 29.00, CategoryID = 3 },
            new() { ProductID = 13, ProductName = "Big Ship", Description = "Is it a boat or a ship. Let this floating vehicle decide by using its artifically intelligent computer brain!", ImagePath = "boatbig.png", UnitPrice = 95.00, CategoryID = 4 },
            new() { ProductID = 14, ProductName = "Paper Boat", Description = "Floating fun for all! This toy boat can be assembled in seconds. Floats for minutes!Some folding required.", ImagePath = "boatpaper.png", UnitPrice = 4.95, CategoryID = 4 },
            new() { ProductID = 15, ProductName = "Sail Boat", Description = "Put this fun toy sail boat in the water and let it go!", ImagePath = "boatsail.png", UnitPrice = 42.95, CategoryID = 4 },
            new() { ProductID = 16, ProductName = "Rocket", Description = "This fun rocket will travel up to a height of 200 feet.", ImagePath = "rocket.png", UnitPrice = 122.95, CategoryID = 5 }
        };

        productDb.Categories.AddRange(categories);
        productDb.Products.AddRange(products);
        productDb.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseBlazorWebFormsComponents();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseAntiforgery();

app.MapGet("/Account/PerformLogin", async (
    string? email,
    string? password,
    string? returnUrl,
    SignInManager<ApplicationUser> signInManager) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Login?error=Email%20and%20password%20are%20required");
    }

    var result = await signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
        return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl)
            ? "/Account/Login?error=Invalid%20login%20attempt"
            : $"/Account/Login?error=Invalid%20login%20attempt&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
});

app.MapGet("/Account/PerformRegister", async (
    string? email,
    string? password,
    string? confirmPassword,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Email%20and%20password%20are%20required");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    var existingUser = await userManager.FindByEmailAsync(email);
    if (existingUser != null)
    {
        return Results.Redirect("/Account/Register?error=User%20already%20exists");
    }

    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        return Results.Redirect("/Account/Register?error=Unable%20to%20register%20user");
    }

    await signInManager.SignInAsync(user, isPersistent: false);
    return Results.Redirect("/");
});

app.MapGet("/Account/Logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

// Minimal API endpoint — handles /AddToCart via HTTP 302 so cart state
// is stored before the redirect and visible to ShoppingCart's CartService.
app.MapGet("/AddToCart", async (
    int productID,
    HttpContext httpContext,
    WingtipToys.Models.ProductContext db,
    WingtipToys.Logic.CartSessionStore cartStore) =>
{
    await httpContext.Session.LoadAsync();
    const string CartIdKey = "WingtipCartId";
    var cartId = httpContext.Session.GetString(CartIdKey);
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString("N");
        httpContext.Session.SetString(CartIdKey, cartId);
        await httpContext.Session.CommitAsync();
    }
    var product = await db.Products.FindAsync(productID);
    if (product != null) cartStore.AddToCart(cartId, product);
    return Results.Redirect("/ShoppingCart");
});

// Remove item from cart via server-side redirect (works without Blazor circuit)
app.MapGet("/RemoveFromCart", async (
    int productID,
    HttpContext httpContext,
    WingtipToys.Logic.CartSessionStore cartStore) =>
{
    await httpContext.Session.LoadAsync();
    const string CartIdKey = "WingtipCartId";
    var cartId = httpContext.Session.GetString(CartIdKey);
    if (!string.IsNullOrEmpty(cartId))
        cartStore.RemoveItem(cartId, productID);
    return Results.Redirect("/ShoppingCart");
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
