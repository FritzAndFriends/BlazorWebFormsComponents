using BlazorWebFormsComponents;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<CatalogService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<UserStoreService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorPage");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAntiforgery();

app.MapGet("/AddToCart", (int productID, CartService cartService) =>
{
    cartService.AddToCart(productID);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/Cart/Update", (int productId, int quantity, CartService cartService) =>
{
    cartService.UpdateQuantity(productId, quantity);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/Cart/Remove", (int productId, CartService cartService) =>
{
    cartService.Remove(productId);
    return Results.Redirect("/ShoppingCart");
});

app.MapGet("/Account/PerformRegister", (string? email, string? password, string? confirmPassword, UserStoreService userStore) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Email%20and%20password%20are%20required");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    return userStore.Register(email, password, out var registerError)
        ? Results.Redirect("/Account/Login?registered=1")
        : Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(registerError ?? "Registration failed")}");
});

app.MapGet("/Account/PerformLogin", (string? email, string? password, UserStoreService userStore) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Login?error=Email%20and%20password%20are%20required");
    }

    return userStore.Login(email, password, out var loginError)
        ? Results.Redirect("/")
        : Results.Redirect($"/Account/Login?error={Uri.EscapeDataString(loginError ?? "Invalid login")}");
});

app.MapGet("/Account/Logout", (UserStoreService userStore) =>
{
    userStore.Logout();
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();
