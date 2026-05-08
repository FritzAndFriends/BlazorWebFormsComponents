using System.Security.Claims;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WingtipToys.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAntiforgery();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<CartStore>();
builder.Services.AddSingleton<DemoUserStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorPage");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/Account/RegisterHandler", async (HttpContext context, DemoUserStore users) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.Redirect("/Account/Register?error=Passwords%20do%20not%20match");
    }

    if (!users.TryRegister(email, password, out var error))
    {
        return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(error)}");
    }

    return Results.Redirect("/Account/Login?registered=1");
});

app.MapPost("/Account/LoginHandler", async (HttpContext context, DemoUserStore users) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();

    if (!users.ValidateCredentials(email, password))
    {
        var errorUrl = string.IsNullOrWhiteSpace(returnUrl)
            ? "/Account/Login?error=Invalid%20login%20attempt"
            : $"/Account/Login?error=Invalid%20login%20attempt&returnUrl={Uri.EscapeDataString(returnUrl)}";
        return Results.Redirect(errorUrl);
    }

    var principal = new ClaimsPrincipal(new ClaimsIdentity(
    [
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Email, email)
    ], CookieAuthenticationDefaults.AuthenticationScheme));

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
});

app.MapPost("/Account/LogoutHandler", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();
