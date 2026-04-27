using BlazorWebFormsComponents;
using System.Text.Json;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<UserStoreService>();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAntiforgery();
app.UseConfigurationManagerShim();
app.UseBlazorWebFormsComponents();

app.MapPost("/Account/RegisterHandler", async (HttpContext context, UserStoreService users) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString().Trim();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(email))
    {
        return Results.Redirect("/Account/Register?error=Email%20is%20required.");
    }

    if (string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/Account/Register?error=Password%20is%20required.");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match.");
    }

    if (!users.TryRegister(email, password))
    {
        return Results.Redirect("/Account/Register?error=An%20account%20with%20that%20email%20already%20exists.");
    }

    context.Session.SetString(UserStoreService.CurrentUserSessionKey, JsonSerializer.Serialize(email));
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/Account/LoginHandler", async (HttpContext context, UserStoreService users) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString().Trim();
    var password = form["password"].ToString();

    if (!users.ValidateCredentials(email, password))
    {
        return Results.Redirect("/Account/Login?error=Invalid%20login%20attempt");
    }

    context.Session.SetString(UserStoreService.CurrentUserSessionKey, JsonSerializer.Serialize(email));
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapGet("/Account/Logout", (HttpContext context) =>
{
    context.Session.Remove(UserStoreService.CurrentUserSessionKey);
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();
