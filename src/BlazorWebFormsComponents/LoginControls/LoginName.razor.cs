using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginName : BaseWebFormsComponent, IHasStyle
	{
		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Parameter]
		public string FormatString { get; set; } = "{0}";

		public StyleBuilder CalculatedStyle => this.ToStyle();

		private bool UserAuthenticated { get; set; }

		private string DisplayName { get; set; }

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
