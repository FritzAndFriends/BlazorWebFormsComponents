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
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContosoUniversity")));

builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Enrollmet_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();

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
