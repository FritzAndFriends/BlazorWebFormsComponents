using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;

using ContosoUniversity.BLL;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

var connectionString = builder.Configuration.GetConnectionString("ContosoUniversityEntities")
    ?? throw new InvalidOperationException("Connection string 'ContosoUniversityEntities' was not found.");
builder.Services.AddDbContext<global::ContosoUniversity.Models.ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));


// Service classes discovered with constructor injection — registered for DI
builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Enrollmet_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Ensure database tables exist for all registered DbContexts
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<global::ContosoUniversity.Models.ContosoUniversityEntities>().Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseBlazorWebFormsComponents();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>();

app.Run();
