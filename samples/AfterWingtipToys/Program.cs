using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ProductContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartStateService>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Seed the database
{
    var factory = app.Services.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();
    ProductDatabaseInitializer.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Auth endpoints — plain HTML forms post here
app.MapPost("/account/register-handler", async (HttpContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        return Results.Redirect("/Account/Register?error=Email+and+password+are+required");
    }
    if (password != confirmPassword)
    {
        return Results.Redirect("/Account/Register?error=Passwords+do+not+match");
    }

    var user = new IdentityUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Redirect(form["ReturnUrl"].ToString() is { Length: > 0 } returnUrl ? returnUrl : "/");
    }

    var errors = string.Join("+", result.Errors.Select(e => e.Description));
    return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(errors)}");
});

app.MapPost("/account/login-handler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();
    var rememberMe = form["RememberMe"].ToString() == "true";

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (result.Succeeded)
    {
        return Results.Redirect(form["ReturnUrl"].ToString() is { Length: > 0 } returnUrl ? returnUrl : "/");
    }

    return Results.Redirect("/Account/Login?error=Invalid+login+attempt");
});

app.MapPost("/account/logout-handler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapGet("/AddToCart", async (int? productID, CartStateService cartService) =>
{
    if (productID.HasValue && productID > 0)
    {
        await cartService.AddToCartAsync(productID.Value);
    }
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/RemoveFromCart", async (string? itemId, CartStateService cartService) =>
{
    if (!string.IsNullOrEmpty(itemId))
    {
        await cartService.RemoveItemAsync(itemId);
    }
    return Results.Redirect("/ShoppingCart");
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

