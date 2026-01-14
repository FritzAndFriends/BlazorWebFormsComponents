using AfterBlazorServerSide;
using AfterBlazorServerSide.Components;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseStaticWebAssets();

builder.Services.AddBlazorWebFormsComponents();

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebFormsComponents.IStyle).Assembly);

await app.RunAsync();

partial class Program
{
    public static string ApplicationName => "Blazor Server-Side";
    public static string? ComponentVersion = typeof(BlazorWebFormsComponents.Button)
        .Assembly
        .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion
        ?.Split('+')[0]; // Gets "0.13.0" without the commit hash
}
