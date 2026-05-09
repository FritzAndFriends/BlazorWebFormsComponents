using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=identity.db";
var productConnectionString = builder.Configuration.GetConnectionString("WingtipToys")
    ?? "Data Source=wingtiptoys.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(identityConnectionString));
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite(productConnectionString), ServiceLifetime.Scoped);
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IDbContextFactory<ProductContext>>().CreateDbContext());

var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
});
identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.UseConfigurationManagerShim();
await EnsureSeedDataAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorPage");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseBlazorWebFormsComponents();

app.MapGet("/AddToCart", async (int productId, HttpContext ctx) =>
{
    var cartId = ctx.Session.GetString("CartId");
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        ctx.Session.SetString("CartId", cartId);
    }

    using var scope = ctx.RequestServices.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
    var item = await db.ShoppingCartItems.FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
    if (item != null)
    {
        item.Quantity++;
    }
    else
    {
        db.ShoppingCartItems.Add(new CartItem
        {
            ItemId = Guid.NewGuid().ToString(),
            CartId = cartId,
            ProductId = productId,
            Quantity = 1,
            DateCreated = DateTime.UtcNow
        });
    }

    await db.SaveChangesAsync();
    return Results.LocalRedirect("/ShoppingCart");
});

app.MapGet("/RemoveFromCart", async (int productId, HttpContext ctx) =>
{
    var cartId = ctx.Session.GetString("CartId");
    if (!string.IsNullOrEmpty(cartId))
    {
        using var scope = ctx.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
        var item = await db.ShoppingCartItems.FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
        if (item != null)
        {
            db.ShoppingCartItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }

    return Results.LocalRedirect("/ShoppingCart");
});

app.MapGet("/Account/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
    return Results.LocalRedirect("/");
});

app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var rememberMe = string.Equals(form["RememberMe"], "on", StringComparison.OrdinalIgnoreCase)
        || string.Equals(form["RememberMe"], "true", StringComparison.OrdinalIgnoreCase);
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
        var invalidLoginUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Invalid%20login%20attempt&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Invalid%20login%20attempt";
        return Results.LocalRedirect(invalidLoginUrl);
    }

    return Results.LocalRedirect(hasLocalReturnUrl ? returnUrl : "/");
});

app.MapPost("/Account/RegisterHandler", async (HttpContext context, UserManager<ApplicationUser> userManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        var mismatchedPasswordUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Passwords%20do%20not%20match&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Passwords%20do%20not%20match";
        return Results.LocalRedirect(mismatchedPasswordUrl);
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
        var registrationErrorUrl = hasLocalReturnUrl
            ? $"/Account/Register?error={Uri.EscapeDataString(error)}&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : $"/Account/Register?error={Uri.EscapeDataString(error)}";
        return Results.LocalRedirect(registrationErrorUrl);
    }

    var registeredUrl = hasLocalReturnUrl
        ? $"/Account/Login?registered=1&returnUrl={Uri.EscapeDataString(returnUrl)}"
        : "/Account/Login?registered=1";
    return Results.LocalRedirect(registeredUrl);
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task EnsureSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await identityDb.Database.EnsureCreatedAsync();

    var productDb = scope.ServiceProvider.GetRequiredService<ProductContext>();
    await productDb.Database.EnsureCreatedAsync();

    if (await productDb.Categories.AnyAsync())
    {
        return;
    }

    var categories = new[]
    {
        new Category { CategoryID = 1, CategoryName = "Cars", Description = "Toy cars" },
        new Category { CategoryID = 2, CategoryName = "Planes", Description = "Toy planes" },
        new Category { CategoryID = 4, CategoryName = "Boats", Description = "Toy boats" },
        new Category { CategoryID = 5, CategoryName = "Rockets", Description = "Toy rockets" }
    };

    var products = new[]
    {
        new Product { ProductID = 1, ProductName = "Convertible Car", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!", ImagePath = "carconvert.png", UnitPrice = 22.50, CategoryID = 1 },
        new Product { ProductID = 2, ProductName = "Old-time Car", Description = "There's nothing old about this toy car, except its looks.", ImagePath = "carearly.png", UnitPrice = 15.95, CategoryID = 1 },
        new Product { ProductID = 3, ProductName = "Fast Car", Description = "Yes this car is fast, but it also floats in water.", ImagePath = "carfast.png", UnitPrice = 32.99, CategoryID = 1 },
        new Product { ProductID = 4, ProductName = "Super Fast Car", Description = "Use this super fast car to entertain guests. Lights and doors work!", ImagePath = "carfaster.png", UnitPrice = 8.95, CategoryID = 1 },
        new Product { ProductID = 5, ProductName = "Old Style Racer", Description = "This old style racer can fly (with user assistance).", ImagePath = "carracer.png", UnitPrice = 34.95, CategoryID = 1 },
        new Product { ProductID = 6, ProductName = "Ace Plane", Description = "Authentic airplane toy. Features realistic color and details.", ImagePath = "planeace.png", UnitPrice = 95.00, CategoryID = 2 },
        new Product { ProductID = 7, ProductName = "Glider", Description = "This fun glider is made from real balsa wood.", ImagePath = "planeglider.png", UnitPrice = 4.95, CategoryID = 2 },
        new Product { ProductID = 8, ProductName = "Paper Plane", Description = "This paper plane is like no other paper plane.", ImagePath = "planepaper.png", UnitPrice = 2.95, CategoryID = 2 },
        new Product { ProductID = 9, ProductName = "Propeller Plane", Description = "Rubber band powered plane features two wheels.", ImagePath = "planeprop.png", UnitPrice = 32.95, CategoryID = 2 },
        new Product { ProductID = 13, ProductName = "Big Ship", Description = "Is it a boat or a ship? Let this floating vehicle decide.", ImagePath = "boatbig.png", UnitPrice = 95.00, CategoryID = 4 },
        new Product { ProductID = 14, ProductName = "Paper Boat", Description = "Floating fun for all! Some folding required.", ImagePath = "boatpaper.png", UnitPrice = 4.95, CategoryID = 4 },
        new Product { ProductID = 15, ProductName = "Sail Boat", Description = "Put this fun toy sail boat in the water and let it go!", ImagePath = "boatsail.png", UnitPrice = 42.95, CategoryID = 4 },
        new Product { ProductID = 16, ProductName = "Rocket", Description = "This fun rocket will travel up to a height of 200 feet.", ImagePath = "rocket.png", UnitPrice = 122.95, CategoryID = 5 }
    };

    await productDb.Categories.AddRangeAsync(categories);
    await productDb.Products.AddRangeAsync(products);
    await productDb.SaveChangesAsync();
}
