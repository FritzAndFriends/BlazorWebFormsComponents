using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WingtipToys.Data;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// BWFC requires IHttpContextAccessor — register before AddBlazorWebFormsComponents
builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazorWebFormsComponents();

// EF Core with SQLite
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

// Shopping cart services
builder.Services.AddScoped<CartStateService>();
builder.Services.AddScoped<ShoppingCartService>();

// Auth services (mock for migration benchmark — scoped with cookie-based persistence)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");
builder.Services.AddSingleton<MockAuthService>();
builder.Services.AddScoped<MockAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MockAuthenticationStateProvider>());
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAntiforgery();

// AddToCart as a minimal API endpoint (avoids Blazor enhanced navigation issues)
app.MapGet("/AddToCart", async (int? productID, ShoppingCartService cartService) =>
{
    if (productID.HasValue && productID > 0)
    {
        await cartService.AddToCartAsync(productID.Value);
    }
    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();

// RemoveAllCartItems — HTTP endpoint so Playwright detects the navigation
app.MapGet("/RemoveAllCartItems", async (HttpContext context, IDbContextFactory<ProductContext> dbFactory) =>
{
    var cartId = context.Request.Cookies["CartId"];
    if (!string.IsNullOrEmpty(cartId))
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var items = await db.ShoppingCartItems.Where(c => c.CartId == cartId).ToListAsync();
        db.ShoppingCartItems.RemoveRange(items);
        await db.SaveChangesAsync();
    }
    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();

// Register — HTTP POST (POST form submissions bypass Blazor's Navigation API interception)
app.MapPost("/Account/DoRegister", async (HttpContext context, MockAuthService authService) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
    {
        await authService.CreateUserAsync(email, password);
    }
    return Results.Redirect("/Account/Login");
}).DisableAntiforgery();

// Login — HTTP POST with cookie authentication
app.MapPost("/Account/DoLogin", async (HttpContext context, MockAuthService authService) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password)
        && await authService.AuthenticateAsync(email, password))
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
