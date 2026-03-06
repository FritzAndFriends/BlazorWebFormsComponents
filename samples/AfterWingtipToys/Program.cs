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

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
