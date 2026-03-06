using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddHttpContextAccessor();

// EF Core with SQLite
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

// ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ProductContext>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
    db.Database.EnsureCreated();
    ProductDatabaseInitializer.Initialize(db);
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

// AddToCart endpoint — adds product to session cart and redirects to ShoppingCart
app.MapGet("/AddToCart", (int? productID, ShoppingCartService cart) =>
{
    if (productID.HasValue && productID > 0)
        cart.AddToCart(productID.Value);
    return Results.Redirect("/ShoppingCart");
});

// Cart Update endpoint — updates item quantity and redirects back
app.MapPost("/Cart/Update", async (HttpContext context, ShoppingCartService cart) =>
{
    var form = await context.Request.ReadFormAsync();
    var itemId = form["itemId"].ToString();
    if (int.TryParse(form["quantity"], out var qty) && qty > 0)
        cart.UpdateItem(itemId, qty);
    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();

// Cart Remove endpoint — removes item and redirects back
app.MapPost("/Cart/Remove", async (HttpContext context, ShoppingCartService cart) =>
{
    var form = await context.Request.ReadFormAsync();
    var itemId = form["itemId"].ToString();
    cart.RemoveItem(itemId);
    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();

// Login POST endpoint — authenticates user and sets auth cookie via HTTP response
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    if (result.Succeeded)
        return Results.Redirect("/");
    return Results.Redirect("/Account/Login?error=Invalid+login+attempt");
}).DisableAntiforgery();

// Register POST endpoint — creates user, signs in, and redirects
app.MapPost("/Account/RegisterHandler", async (HttpContext context,
    UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    var user = new IdentityUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Redirect("/");
    }
    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
    return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(errors)}");
}).DisableAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
