using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ContosoUniversityContext>(options =>
    options.UseSqlite("Data Source=contoso.db"));

builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<Enrollmet_Logic>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ContosoUniversityContext>>();
    using var context = factory.CreateDbContext();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();