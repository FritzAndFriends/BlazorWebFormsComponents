using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("WingtipToys")
    ?? throw new InvalidOperationException("Connection string 'WingtipToys' was not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(connectionString));

var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>();
identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Ensure database tables exist for all registered DbContexts
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
    var productContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using (var productContext = productContextFactory.CreateDbContext())
    {
        productContext.Database.EnsureCreated();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// TODO(bwfc-general): Review legacy Application_Start registrations:
// - RouteConfig.RegisterRoutes
// - BundleConfig.RegisterBundles
// - Database.SetInitializer
// - Application_Error

// Custom error page detected from Web.config <customErrors defaultRedirect="ErrorPage.aspx?handler=customErrors%20section%20-%20Web.config">
// The UseExceptionHandler("/Error") above handles this. Create an /Error page if needed.

// --- BWFC generated handler stubs ---
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var rememberMe = string.Equals(form["RememberMe"], "on", StringComparison.OrdinalIgnoreCase)
        || string.Equals(form["RememberMe"], "true", StringComparison.OrdinalIgnoreCase);
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
        var invalidLoginUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Invalid%20login%20attempt&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Invalid%20login%20attempt";
        return Results.LocalRedirect(invalidLoginUrl);
    }

    return Results.LocalRedirect(hasLocalReturnUrl ? returnUrl : "/");
}).DisableAntiforgery();

app.MapPost("/Account/RegisterHandler", async (HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        var mismatchedPasswordUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Passwords%20do%20not%20match&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Passwords%20do%20not%20match";
        return Results.LocalRedirect(mismatchedPasswordUrl);
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
        var registrationErrorUrl = hasLocalReturnUrl
            ? $"/Account/Register?error={Uri.EscapeDataString(error)}&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : $"/Account/Register?error={Uri.EscapeDataString(error)}";
        return Results.LocalRedirect(registrationErrorUrl);
    }

    await signInManager.SignInAsync(user, isPersistent: false);
    return Results.LocalRedirect(hasLocalReturnUrl ? returnUrl : "/");
}).DisableAntiforgery();

app.MapPost("/Account/PerformLogout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

// Seed identity roles and users (detected from Web Forms source)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("canEdit"))
        await roleManager.CreateAsync(new IdentityRole("canEdit"));
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userManager.FindByEmailAsync("canEditUser@wingtiptoys.com") == null)
    {
        var seedUser = new ApplicationUser { UserName = "canEditUser@wingtiptoys.com", Email = "canEditUser@wingtiptoys.com" };
        await userManager.CreateAsync(seedUser, "Pa$$word1");
        await userManager.AddToRoleAsync(seedUser, "canEdit");
    }
}

app.Run();
