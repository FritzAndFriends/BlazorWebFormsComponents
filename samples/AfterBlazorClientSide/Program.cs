using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<AfterBlazorClientSide.App>("#app");

builder.Services.AddScoped<AuthenticationStateProvider, AfterBlazorClientSide.StaticAuthStateProvider>();
builder.Services.AddSingleton(
	new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
);

await builder.Build().RunAsync();

partial class Program
{
	public static string ApplicationName => "Blazor WebAssembly";
}
