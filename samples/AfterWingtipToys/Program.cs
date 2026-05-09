using BlazorWebFormsComponents;
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

var authConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
var catalogConnectionString = builder.Configuration.GetConnectionString("WingtipToys")
    ?? throw new InvalidOperationException("Connection string 'WingtipToys' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(authConnectionString));
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlite(catalogConnectionString));

var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var identityDb = services.GetRequiredService<ApplicationDbContext>();
    var productDb = services.GetRequiredService<ProductContext>();

    await identityDb.Database.EnsureCreatedAsync();
    await productDb.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(productDb);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorPage");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

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

app.MapPost("/Account/PerformLogout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/");
});

app.MapGet("/AddToCart", async (int? productID, HttpContext context, ProductContext db) =>
{
    if (productID.HasValue)
    {
        var cartId = context.Session.GetString("CartId");
        if (string.IsNullOrEmpty(cartId))
        {
            cartId = Guid.NewGuid().ToString();
            context.Session.SetString("CartId", cartId);
        }

        var cartItem = await db.ShoppingCartItems
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productID.Value);

        if (cartItem is null)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.ProductID == productID.Value);
            if (product is not null)
            {
                db.ShoppingCartItems.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = cartId,
                    ProductId = product.ProductID,
                    Product = product,
                    Quantity = 1,
                    DateCreated = DateTime.UtcNow
                });
            }
        }
        else
        {
            cartItem.Quantity++;
        }
        await db.SaveChangesAsync();
    }
    return Results.LocalRedirect("/ShoppingCart");
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
