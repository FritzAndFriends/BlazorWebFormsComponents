using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazorWebFormsComponents();

// AuthorizeView (converted from LoginView) requires cascading auth state
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));
builder.Services.AddScoped<CartStateService>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var context = factory.CreateDbContext();
    ProductDatabaseInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
