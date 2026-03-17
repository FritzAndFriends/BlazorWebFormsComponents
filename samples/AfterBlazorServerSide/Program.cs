global using BlazorWebFormsComponents;
using AfterBlazorServerSide;
using AfterBlazorServerSide.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseStaticWebAssets();

builder.Services.AddBlazorWebFormsComponents();

// Register the Component Health Dashboard diagnostic service.
// Navigate from the sample app content root up to the repository root.
var solutionRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".."));
builder.Services.AddComponentHealthDashboard(solutionRoot);

var services = builder.Services;

services.AddRazorComponents()
    .AddInteractiveServerComponents();
services.AddScoped<AuthenticationStateProvider, StaticAuthStateProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseBlazorWebFormsComponents();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebFormsComponents.IStyle).Assembly);

await app.RunAsync();

public partial class Program
{
    public static string ApplicationName => "Blazor Server-Side";
    public static string? ComponentVersion = typeof(BlazorWebFormsComponents.Button)
        .Assembly
        .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion
        ?.Split('+')[0]; // Gets version without the commit hash
}
