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

// Redirect legacy .aspx URLs to clean Blazor routes so existing bookmarks and links keep working.
// /Home.aspx → /   (Home is the root route)
// /About.aspx → /About, /Students.aspx → /Students, etc.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
    {
        var cleanPath = path[..^5]; // strip .aspx
        if (string.Equals(cleanPath, "/Home", StringComparison.OrdinalIgnoreCase))
            cleanPath = "/";
        context.Response.Redirect(cleanPath + context.Request.QueryString, permanent: true);
        return;
    }
    await next(context);
});

app.MapStaticAssets();
app.UseBlazorWebFormsComponents();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>();

app.Run();
