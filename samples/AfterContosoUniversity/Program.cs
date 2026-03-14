using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using ContosoUniversity.Bll;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ContosoUniversityEntities>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ContosoUniversity;Trusted_Connection=True;"));

builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddScoped<Enrollmet_Logic>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.UseBlazorWebFormsComponents();
app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
