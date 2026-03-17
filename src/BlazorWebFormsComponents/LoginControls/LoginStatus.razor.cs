using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using static BlazorWebFormsComponents.Enums.LogoutAction;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginStatus : BaseStyledComponent
	{
		[Parameter] public LogoutAction LogoutAction { get; set; } = Refresh;

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		[Parameter] public string LoginText { get; set; } = "Login";

		[Parameter] public string LoginImageUrl { get; set; }

		// Blazor adaptation: Web Forms LoginStatus used FormsAuthentication.LoginUrl from web.config,
		// which doesn't exist in Blazor. This parameter lets the consumer specify the login page URL directly.
		[Parameter] public string LoginPageUrl { get; set; }

		[Parameter] public string LogoutText { get; set; } = "Logout";

		[Parameter] public string LogoutImageUrl { get; set; }

		[Parameter] public string LogoutPageUrl { get; set; }



		#region

		[Parameter] public EventCallback<LoginCancelEventArgs> OnLoggingOut { get; set; }

		[Parameter] public EventCallback<EventArgs> OnLoggedOut { get; set; }

		#endregion

		private void LoginHandle(MouseEventArgs args)
		{

			if (!string.IsNullOrEmpty(LoginPageUrl))
			{
				NavigationManager.NavigateTo(LoginPageUrl);
			}

		}

		private async Task LogoutHandle(MouseEventArgs args)
		{

			var logoutCancelEventArgs = new LoginCancelEventArgs() { Sender = this };
			await OnLoggingOut.InvokeAsync(logoutCancelEventArgs);

			if (!logoutCancelEventArgs.Cancel)
			{

				await OnLoggedOut.InvokeAsync(EventArgs.Empty);

				if (LogoutAction == Redirect)
				{

					NavigationManager.NavigateTo(LogoutPageUrl);

				}
			}

		}

	}

}
