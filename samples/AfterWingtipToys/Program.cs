using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

// Database — factory only (no dual registration)
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ProductContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Login";
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var context = factory.CreateDbContext();
    context.Database.EnsureCreated();
    ProductDatabaseInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Cart endpoints — SSR with cookies
app.MapGet("/AddToCart", async (int productId, HttpContext httpContext, IDbContextFactory<ProductContext> dbFactory) =>
{
    using var db = dbFactory.CreateDbContext();
    var cartId = httpContext.Request.Cookies["WingtipToysCartId"];
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        httpContext.Response.Cookies.Append("WingtipToysCartId", cartId, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30), HttpOnly = true, IsEssential = true });
    }

    var product = await db.Products.FindAsync(productId);
    if (product != null)
    {
        var existingItem = await db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            db.ShoppingCartItems.Add(new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                CartId = cartId,
                ProductId = productId,
                Quantity = 1,
                DateCreated = DateTime.Now
            });
        }
        await db.SaveChangesAsync();
    }

    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/RemoveFromCart", async (string itemId, HttpContext httpContext, IDbContextFactory<ProductContext> dbFactory) =>
{
    using var db = dbFactory.CreateDbContext();
    var cartId = httpContext.Request.Cookies["WingtipToysCartId"];
    if (!string.IsNullOrEmpty(cartId))
    {
        var item = await db.ShoppingCartItems.FirstOrDefaultAsync(c => c.ItemId == itemId && c.CartId == cartId);
        if (item != null)
        {
            db.ShoppingCartItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }
    return Results.Redirect("/ShoppingCart");
});

// Auth endpoints
app.MapPost("/account/register-handler", async (HttpContext httpContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();

    var user = new IdentityUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Redirect("/");
    }

    var errors = string.Join(",", result.Errors.Select(e => e.Description));
    return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(errors)}");
});

app.MapPost("/account/login-handler", async (HttpContext httpContext, SignInManager<IdentityUser> signInManager) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();
    var rememberMe = form["RememberMe"].ToString() == "on";

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        return Results.Redirect("/");
    }

    return Results.Redirect("/Account/Login?error=Invalid+login+attempt");
});

app.MapGet("/account/logout-handler", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

