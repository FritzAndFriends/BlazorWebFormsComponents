// TODO: Review and adjust this generated Program.cs for your application needs.
using BlazorWebFormsComponents;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

// Register the DbContext with SQL Server LocalDB
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "ContosoUniversity.mdf");
var connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;Connect Timeout=30";

// Set connection string for BLL classes that use new ContosoUniversityEntities()
ContosoUniversityEntities.SetConnectionString(connectionString);

builder.Services.AddDbContext<ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseBlazorWebFormsComponents();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
