using WingtipToys.Data;
using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));

builder.Services.AddHttpContextAccessor();

// Minimal auth services so pages using AuthorizeView/CascadingAuthenticationState don't crash.
// Full Identity is not yet configured — all users appear anonymous.
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication().AddCookie();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductContext>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<WingtipToys.Components.App>();

app.Run();
