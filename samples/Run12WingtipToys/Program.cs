using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

builder.Services.AddScoped<CartStateService>();

// Auth services (mock for migration benchmark)
builder.Services.AddScoped<MockAuthService>();
builder.Services.AddScoped<MockAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MockAuthenticationStateProvider>());
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProductContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();
    ProductContext.Seed(db);
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
