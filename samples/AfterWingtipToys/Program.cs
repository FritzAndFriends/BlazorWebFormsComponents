using System.Security.Claims;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<UserAccountStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/Account/PerformRegister", async (HttpContext context, UserAccountStore userStore) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    if (!userStore.Register(email, password, out var error))
    {
        return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(error ?? "Registration%20failed")}");
    }

    return Results.Redirect("/Account/Login?registered=1");
}).DisableAntiforgery();

app.MapPost("/Account/PerformLogin", async (HttpContext context, UserAccountStore userStore) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();

    if (!userStore.Validate(email, password))
    {
        return Results.Redirect($"/Account/Login?error={Uri.EscapeDataString("Invalid login attempt")}");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, email),
        new(ClaimTypes.Email, email)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
}).DisableAntiforgery();

app.MapGet("/Account/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapGet("/AddToCart", (HttpContext context, CartService cartService) =>
{
    if (!int.TryParse(context.Request.Query["productID"], out var productId))
    {
        return Results.Redirect("/ProductList");
    }

    var cartId = GetCartId(context);
    cartService.AddToCart(cartId, productId);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/RemoveFromCart", (HttpContext context, CartService cartService) =>
{
    if (int.TryParse(context.Request.Query["productID"], out var productId))
    {
        cartService.RemoveFromCart(GetCartId(context), productId);
    }

    return Results.Redirect("/ShoppingCart");
});

app.MapPost("/ShoppingCart/Update", async (HttpContext context, CartService cartService) =>
{
    var cartId = GetCartId(context);
    var form = await context.Request.ReadFormAsync();
    foreach (var entry in form)
    {
        if (!entry.Key.StartsWith("qty_", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (!int.TryParse(entry.Key[4..], out var productId))
        {
            continue;
        }

        if (int.TryParse(entry.Value.ToString(), out var quantity))
        {
            cartService.UpdateQuantity(cartId, productId, quantity);
        }
    }

    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();

static string GetCartId(HttpContext context)
{
    const string cartSessionKey = "CartId";

    if (!string.IsNullOrWhiteSpace(context.User.Identity?.Name))
    {
        context.Session.SetString(cartSessionKey, context.User.Identity.Name);
        return context.User.Identity.Name;
    }

    var existingCartId = context.Session.GetString(cartSessionKey);
    if (!string.IsNullOrWhiteSpace(existingCartId))
    {
        return existingCartId;
    }

    var newCartId = Guid.NewGuid().ToString("N");
    context.Session.SetString(cartSessionKey, newCartId);
    return newCartId;
}
