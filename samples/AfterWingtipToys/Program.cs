using BlazorWebFormsComponents;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddSingleton<ProductCatalogService>();
builder.Services.AddSingleton<CartStore>();
builder.Services.AddSingleton<SimpleUserStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/AddToCart", (HttpContext context, CartStore cartStore, ProductCatalogService catalog, int? productId) =>
{
    if (productId is null || catalog.GetProduct(productId, null) is null)
    {
        return Results.Redirect("/ProductList");
    }

    var cartKey = context.Session.GetString("cart-key");
    if (string.IsNullOrWhiteSpace(cartKey))
    {
        cartKey = Guid.NewGuid().ToString();
        context.Session.SetString("cart-key", cartKey);
    }

    cartStore.AddToCart(cartKey, productId.Value, catalog);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/Account/PerformRegister", (HttpContext context, SimpleUserStore users, string? email, string? password, string? confirmPassword) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Email%20and%20password%20are%20required");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    if (!users.TryRegister(email, password))
    {
        return Results.Redirect("/Account/Register?error=An%20account%20with%20that%20email%20already%20exists");
    }

    return Results.Redirect("/Account/Login?registered=1");
});

app.MapGet("/Account/PerformLogin", (HttpContext context, SimpleUserStore users, string? email, string? password, string? returnUrl) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Login?error=Email%20and%20password%20are%20required");
    }

    if (!users.Validate(email, password))
    {
        return Results.Redirect("/Account/Login?error=Invalid%20login%20attempt");
    }

    context.Session.SetString("auth-email", email);
    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
});

app.MapGet("/Account/Logout", (HttpContext context) =>
{
    context.Session.Remove("auth-email");
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();
