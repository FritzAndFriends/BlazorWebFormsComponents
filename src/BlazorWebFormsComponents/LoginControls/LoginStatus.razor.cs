using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using static BlazorWebFormsComponents.Enums.LogoutAction;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginStatus : BaseWebFormsComponent, IHasStyle
	{
		[Parameter] public LogoutAction LogoutAction { get; set; } = Refresh;

		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		[Parameter] public string LoginText { get; set; } = "Login";

		[Parameter] public string LoginImageUrl { get; set; }

		// This property was not in Webforms
		[Parameter] public string LoginPageUrl { get; set; }

		[Parameter] public string LogoutText { get; set; } = "Logout";

		[Parameter] public string LogoutImageUrl { get; set; }

		[Parameter] public string LogoutPageUrl { get; set; }



		#region

		[Parameter] public EventCallback<LoginCancelEventArgs> OnLoggingOut { get; set; }

		[Parameter] public EventCallback<EventArgs> OnLoggedOut { get; set; }

		#endregion

		public StyleBuilder CalculatedStyle => this.ToStyle();

		private bool UserAuthenticated { get; set; }

		#region IHasStyle

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public FontInfo Font { get; set; } = new FontInfo();

		#endregion

		private void LoginHandle(MouseEventArgs args)
		{

			NavigationManager.NavigateTo(LoginPageUrl);

		}

		private async Task LogoutHandle(MouseEventArgs args)
		{

			var logoutCancelEventArgs = new LoginCancelEventArgs();
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



		protected override async Task OnInitializedAsync()
		{

			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			UserAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;

			await base.OnInitializedAsync();

		}

	}

}
