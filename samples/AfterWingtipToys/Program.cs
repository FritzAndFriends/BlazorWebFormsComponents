using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("WingtipToys")
    ?? throw new InvalidOperationException("Connection string 'WingtipToys' was not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(connectionString));

var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>();
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

// Ensure database tables exist and seed product data
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
    var productContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using (var productContext = productContextFactory.CreateDbContext())
    {
        productContext.Database.EnsureCreated();
        if (!productContext.Categories.Any())
        {
            productContext.Categories.AddRange(
                new Category { CategoryID = 1, CategoryName = "Cars" },
                new Category { CategoryID = 2, CategoryName = "Planes" },
                new Category { CategoryID = 3, CategoryName = "Trucks" },
                new Category { CategoryID = 4, CategoryName = "Boats" },
                new Category { CategoryID = 5, CategoryName = "Rockets" });
            productContext.SaveChanges();
        }
        if (!productContext.Products.Any())
        {
            productContext.Products.AddRange(
                new Product { ProductID = 1, ProductName = "Convertible Car", Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!", ImagePath = "carconvert.png", UnitPrice = 22.50, CategoryID = 1 },
                new Product { ProductID = 2, ProductName = "Old-time Car", Description = "There's nothing old about this toy car, except it's looks. Compatible with other old toy cars.", ImagePath = "carearly.png", UnitPrice = 15.95, CategoryID = 1 },
                new Product { ProductID = 3, ProductName = "Fast Car", Description = "Yes this car is fast, but it also floats in water.", ImagePath = "carfast.png", UnitPrice = 32.99, CategoryID = 1 },
                new Product { ProductID = 4, ProductName = "Super Fast Car", Description = "Use this super fast car to entertain guests. Lights and doors work!", ImagePath = "carfaster.png", UnitPrice = 8.95, CategoryID = 1 },
                new Product { ProductID = 5, ProductName = "Old Style Racer", Description = "This old style racer can fly (with user assistance). Gravity controls flight duration. No batteries required.", ImagePath = "carracer.png", UnitPrice = 34.95, CategoryID = 1 },
                new Product { ProductID = 6, ProductName = "Ace Plane", Description = "Authentic airplane toy. Features realistic color and details.", ImagePath = "planeace.png", UnitPrice = 95.00, CategoryID = 2 },
                new Product { ProductID = 7, ProductName = "Glider", Description = "This fun glider is made from real balsa wood. Some assembly required.", ImagePath = "planeglider.png", UnitPrice = 4.95, CategoryID = 2 },
                new Product { ProductID = 8, ProductName = "Paper Plane", Description = "This paper plane is like no other paper plane. Some folding required.", ImagePath = "planepaper.png", UnitPrice = 2.95, CategoryID = 2 },
                new Product { ProductID = 9, ProductName = "Propeller Plane", Description = "Rubber band powered plane features two wheels.", ImagePath = "planeprop.png", UnitPrice = 32.95, CategoryID = 2 },
                new Product { ProductID = 10, ProductName = "Early Truck", Description = "This toy truck has a real gas powered engine. Requires regular tune ups.", ImagePath = "truckearly.png", UnitPrice = 15.00, CategoryID = 3 },
                new Product { ProductID = 11, ProductName = "Fire Truck", Description = "You will have endless fun with this one quarter sized fire truck.", ImagePath = "truckfire.png", UnitPrice = 26.00, CategoryID = 3 },
                new Product { ProductID = 12, ProductName = "Big Truck", Description = "This fun toy truck can be used to tow other trucks that are not as big.", ImagePath = "truckbig.png", UnitPrice = 29.00, CategoryID = 3 },
                new Product { ProductID = 13, ProductName = "Big Ship", Description = "Is it a boat or a ship. Let this floating vehicle decide by using its artifically intelligent computer brain!", ImagePath = "boatbig.png", UnitPrice = 95.00, CategoryID = 4 },
                new Product { ProductID = 14, ProductName = "Paper Boat", Description = "Floating fun for all! This toy boat can be assembled in seconds. Floats for minutes! Some folding required.", ImagePath = "boatpaper.png", UnitPrice = 4.95, CategoryID = 4 },
                new Product { ProductID = 15, ProductName = "Sail Boat", Description = "Put this fun toy sail boat in the water and let it go!", ImagePath = "boatsail.png", UnitPrice = 42.95, CategoryID = 4 },
                new Product { ProductID = 16, ProductName = "Rocket", Description = "This fun rocket will travel up to a height of 200 feet.", ImagePath = "rocket.png", UnitPrice = 122.95, CategoryID = 5 });
            productContext.SaveChanges();
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// TODO(bwfc-general): Review legacy Application_Start registrations:
// - RouteConfig.RegisterRoutes
// - BundleConfig.RegisterBundles
// - Database.SetInitializer
// - Application_Error

// Custom error page detected from Web.config <customErrors defaultRedirect="ErrorPage.aspx?handler=customErrors%20section%20-%20Web.config">
// The UseExceptionHandler("/Error") above handles this. Create an /Error page if needed.

// --- BWFC generated handler stubs ---
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

app.MapPost("/Account/PerformLogout", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/");
}).DisableAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

// Seed identity roles and users (detected from Web Forms source)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("canEdit"))
        await roleManager.CreateAsync(new IdentityRole("canEdit"));
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userManager.FindByEmailAsync("canEditUser@wingtiptoys.com") == null)
    {
        var seedUser = new ApplicationUser { UserName = "canEditUser@wingtiptoys.com", Email = "canEditUser@wingtiptoys.com" };
        await userManager.CreateAsync(seedUser, "Pa$$word1");
        await userManager.AddToRoleAsync(seedUser, "canEdit");
    }
}

app.Run();
