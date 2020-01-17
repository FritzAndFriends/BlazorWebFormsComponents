using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace AfterBlazorServerSide
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var identity = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, "James Bond"),
			}, "Fake authentication type");

			var user = new ClaimsPrincipal(identity);

			return Task.FromResult(new AuthenticationState(user));
		}
	}
}
