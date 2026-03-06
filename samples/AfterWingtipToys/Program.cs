using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));
builder.Services.AddScoped<CartStateService>();
builder.Services.AddScoped<CheckoutStateService>();
builder.Services.AddScoped<IPayPalService, MockPayPalService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ProductContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var context = factory.CreateDbContext();
    ProductDatabaseInitializer.Seed(context);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await IdentityDataSeeder.SeedAsync(roleManager, userManager);
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

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

// HTTP endpoint for login — SignInManager requires an active HTTP response to set cookies,
// which is not available inside a SignalR circuit (InteractiveServer mode).
app.MapGet("/Account/PerformLogin", async (
    string email,
    string password,
    SignInManager<IdentityUser> signInManager) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password,
        isPersistent: false, lockoutOnFailure: false);

    if (result.Succeeded)
        return Results.Redirect("/");
    if (result.IsLockedOut)
        return Results.Redirect("/Account/Lockout");

    return Results.Redirect("/Account/Login?error=" + Uri.EscapeDataString("Invalid login attempt."));
});

// HTTP endpoint for registration — creates user then redirects to login
app.MapGet("/Account/PerformRegister", async (
    string email,
    string password,
    UserManager<IdentityUser> userManager) =>
{
    var user = new IdentityUser { UserName = email, Email = email };
    var createResult = await userManager.CreateAsync(user, password);

    if (createResult.Succeeded)
    {
        return Results.Redirect("/Account/Login");
    }

    var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
    return Results.Redirect("/Account/Register?error=" + Uri.EscapeDataString(errors));
});

// HTTP endpoint for logout — requires an active HTTP response to clear auth cookies
app.MapPost("/Account/PerformLogout", async (
    SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.Run();

