using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AfterBlazorClientSide
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<AuthenticationStateProvider, StaticAuthStateProvider>();
		}

		public void Configure(IComponentsApplicationBuilder app)
		{
			app.AddComponent<App>("app");
		}
	}
}
