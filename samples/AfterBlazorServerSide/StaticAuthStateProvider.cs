﻿using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AfterBlazorServerSide
{
	public class StaticAuthStateProvider : AuthenticationStateProvider
	{

		private static ClaimsPrincipal _user = new();

		public static void Login(string name)
		{

			var identity = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, name),
			}, "static authentication");

			_user = new ClaimsPrincipal(identity);

		}

		public static void Logout()
		{

			_user = new ClaimsPrincipal();

		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{

			return Task.FromResult(new AuthenticationState(_user));

		}
	}
}
