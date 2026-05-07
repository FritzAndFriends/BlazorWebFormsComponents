using BlazorWebFormsComponents;
using WingtipToys.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<CartSessionStore>();
builder.Services.AddSingleton<RegisteredUserStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAuthorization();
app.UseAntiforgery();

static string GetOrCreateCartKey(HttpContext context)
{
    var cartKey = context.Session.GetString("cart-key");
    if (string.IsNullOrWhiteSpace(cartKey))
    {
        cartKey = Guid.NewGuid().ToString("N");
        context.Session.SetString("cart-key", cartKey);
    }

    return cartKey;
}

static string? GetCartKey(HttpContext context) => context.Session.GetString("cart-key");

app.MapGet("/AddToCart", (int? productID, HttpContext context, CatalogService catalog, CartSessionStore carts) =>
{
    if (!productID.HasValue)
    {
        return Results.Redirect("/ProductList");
    }

    var product = catalog.GetProduct(productID.Value, null);
    if (product is null)
    {
        return Results.Redirect("/ProductList");
    }

    carts.AddToCart(GetOrCreateCartKey(context), product);
    return Results.Redirect("/ShoppingCart");
});
app.MapGet("/AddToCart.aspx", (int? productID, HttpContext context, CatalogService catalog, CartSessionStore carts) =>
{
    if (!productID.HasValue)
    {
        return Results.Redirect("/ProductList");
    }

    var product = catalog.GetProduct(productID.Value, null);
    if (product is null)
    {
        return Results.Redirect("/ProductList");
    }

    carts.AddToCart(GetOrCreateCartKey(context), product);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/ShoppingCart/Update", (HttpContext context, CartSessionStore carts) =>
{
    var cartKey = GetCartKey(context);
    if (string.IsNullOrWhiteSpace(cartKey))
    {
        return Results.Redirect("/ShoppingCart");
    }

    foreach (var pair in context.Request.Query)
    {
        if (!pair.Key.StartsWith("qty-", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (int.TryParse(pair.Key[4..], out var productId) && int.TryParse(pair.Value, out var quantity))
        {
            carts.UpdateQuantity(cartKey, productId, quantity);
        }
    }

    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/ShoppingCart/Remove", (int productId, HttpContext context, CartSessionStore carts) =>
{
    var cartKey = GetCartKey(context);
    if (!string.IsNullOrWhiteSpace(cartKey))
    {
        carts.RemoveItem(cartKey, productId);
    }

    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/Account/PerformLogin", (string? email, string? password, string? returnUrl, HttpContext context, RegisteredUserStore users) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Login?error=Email%20and%20password%20are%20required");
    }

    if (!users.Validate(email, password))
    {
        return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl)
            ? "/Account/Login?error=Invalid%20email%20or%20password"
            : $"/Account/Login?error=Invalid%20email%20or%20password&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    context.Session.SetString("current-user-email", email);
    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
});

app.MapGet("/Account/PerformRegister", (string? email, string? password, string? confirmPassword, RegisteredUserStore users) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Email%20and%20password%20are%20required");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    if (!users.Register(email, password, out var error))
    {
        return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(error)}");
    }

    return Results.Redirect("/Account/Login?registered=1");
});

app.MapGet("/Account/Logout", (HttpContext context) =>
{
    context.Session.Remove("current-user-email");
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
