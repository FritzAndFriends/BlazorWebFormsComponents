using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginName : BaseStyledComponent
	{
		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Parameter]
		public string FormatString { get; set; } = "{0}";

		private bool UserAuthenticated { get; set; }

		private string DisplayName { get; set; }

		protected override async Task OnInitializedAsync()
		{

			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			UserAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;

			if (UserAuthenticated)
			{

				DisplayName = string.Format(FormatString, authState.User.Identity.Name);

			}

			await base.OnInitializedAsync();

		}
	}
}
