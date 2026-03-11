// ============================================================================
// TODO: Generate EF Core models from your database using:
// dotnet ef dbcontext scaffold "Server=(localdb)\mssqllocaldb;Database=ContosoUniversityEntities;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context ContosoUniversityEntities --namespace "ContosoUniversityModel" --force
// See scaffold-command.txt for full details and options.
// ============================================================================

// Layer2-transformed
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();  // Required for BWFC GridView/DetailsView
builder.Services.AddBlazorWebFormsComponents();

// Database
builder.Services.AddDbContextFactory<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ASPX URL backward compatibility ΓÇö redirect .aspx URLs to Blazor routes
var rewriteOptions = new RewriteOptions()
    .AddRedirect(@"^Default\.aspx$", "/", statusCode: 301)
    .AddRedirect(@"^(.+)\.aspx$", "$1", statusCode: 301);
app.UseRewriter(rewriteOptions);

app.MapStaticAssets();
app.UseSession();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();


