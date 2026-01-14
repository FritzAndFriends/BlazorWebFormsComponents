using AfterBlazorServerSide;
using AfterBlazorServerSide.Components;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components.Authorization;

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
    public static Version? ComponentVersion = typeof(IStyle).Assembly.GetName().Version;
}
