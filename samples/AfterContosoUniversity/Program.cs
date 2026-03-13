using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ContosoUniversityEntities>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ContosoUniversity;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddScoped<EnrollmentLogic>();
builder.Services.AddScoped<CoursesLogic>();
builder.Services.AddScoped<InstructorsLogic>();
builder.Services.AddScoped<StudentsLogic>();

var app = builder.Build();

// Ensure the database is created (creates tables if LocalDB exists but DB doesn't)
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ContosoUniversityEntities>>();
    using var context = factory.CreateDbContext();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorWebFormsComponents();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
