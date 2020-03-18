using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AfterBlazorClientSide
{
	public class Program
	{
		public static async Task Main(string[] args)
		{

			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddScoped<AuthenticationStateProvider, StaticAuthStateProvider>();
			builder.Services.AddBaseAddressHttpClient();

			await builder.Build().RunAsync();

		}

	}

	public static class Startup
	{

		public static string ApplicationName => "Blazor WebAssembly";

	}

}
