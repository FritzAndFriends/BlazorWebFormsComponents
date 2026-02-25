using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class Login : BaseStyledComponent
	{
		#region Obsolete Attributes / Properties

		/// <summary>
		/// 🚨🚨 Use OnAuthenticate event to implement an authentication system 🚨🚨
		/// </summary>
		[Parameter, Obsolete("MembershipProvider not supported in blazor")]
		public string MembershipProvider { get; set; }

		#endregion

		#region Not implemented yet

		//[Parameter] public LoginFailureAction FailureAction { get; set; }
		//[Parameter] public ITemplate LayoutTemplate { get; set; }
		//[Parameter] public bool RenderOuterTable { get; set; } = true;

		#endregion

		#region Layout

		[Parameter] public Orientation Orientation { get; set; } = Orientation.Vertical;
		[Parameter] public LoginTextLayout TextLayout { get; set; } = LoginTextLayout.TextOnLeft;

		#endregion


		[Parameter] public string LoginButtonImageUrl { get; set; }
		[Parameter] public string LoginButtonText { get; set; } = "Log In";
		[Parameter] public ButtonType LoginButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string Password { get => Model?.Password ?? string.Empty; set => Model.Password = value; }
		[Parameter] public string PasswordLabelText { get; set; } = "Password:";
		[Parameter] public string PasswordRecoveryText { get; set; }
		[Parameter] public string PasswordRecoveryUrl { get; set; }
		[Parameter] public string PasswordRecoveryIconUrl { get; set; }
		[Parameter] public string PasswordRequiredErrorMessage { get; set; } = "Password is required.";
		[Parameter] public bool RememberMeSet { get => Model?.RememberMe ?? false; set => Model.RememberMe = value; }
		[Parameter] public string RememberMeText { get; set; } = "Remember me next time.";
		[Parameter] public string TitleText { get; set; } = "Log In";
		[Parameter] public string UserName { get => Model?.Username ?? string.Empty; set => Model.Username = value; }
		[Parameter] public string UserNameLabelText { get; set; } = "User Name:";
		[Parameter] public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";
		[Parameter] public bool VisibleWhenLoggedIn { get; set; } = true;
		[Parameter] public string FailureText { get; set; } = "Your login attempt was not successful. Please try again.";
		[Parameter] public int BorderPadding { get; set; } = 1;
		[Parameter] public string CreateUserText { get; set; }
		[Parameter] public string CreateUserUrl { get; set; }
		[Parameter] public string CreateUserIconUrl { get; set; }
		[Parameter] public string DestinationPageUrl { get; set; }
		[Parameter] public bool DisplayRememberMe { get; set; } = true;
		[Parameter] public string HelpPageText { get; set; }
		[Parameter] public string HelpPageUrl { get; set; }
		[Parameter] public string HelpPageIconUrl { get; set; }
		[Parameter] public string InstructionText { get; set; }

		#region Events

		[Parameter] public EventCallback<LoginCancelEventArgs> OnLoggingIn { get; set; }
		[Parameter] public EventCallback<EventArgs> OnLoginError { get; set; }
		[Parameter] public EventCallback<EventArgs> OnLoggedIn { get; set; }
		[Parameter] public EventCallback<AuthenticateEventArgs> OnAuthenticate { get; set; }

		#endregion

		#region Style

		[Parameter] public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name = "FailureTextStyle")]
		private TableItemStyle FailureTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "TitleTextStyle")]
		private TableItemStyle TitleTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "LabelStyle")]
		private TableItemStyle LabelStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "CheckBoxStyle")]
		private TableItemStyle CheckBoxStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "HyperLinkStyle")]
		private TableItemStyle HyperLinkStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "InstructionTextStyle")]
		private TableItemStyle InstructionTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "TextBoxStyle")]
		private Style TextBoxStyle { get; set; } = new Style();

		[CascadingParameter(Name = "LoginButtonStyle")]
		private Style LoginButtonStyle { get; set; } = new Style();

		[CascadingParameter(Name = "ValidatorTextStyle")]
		private Style ValidatorTextStyle { get; set; } = new Style();

		#endregion

		private bool HasHelp => !string.IsNullOrEmpty(HelpPageText) || !string.IsNullOrEmpty(HelpPageIconUrl);
		private bool HasPasswordRevocery => !string.IsNullOrEmpty(PasswordRecoveryText) || !string.IsNullOrEmpty(PasswordRecoveryIconUrl);
		private bool HasCreateUser => !string.IsNullOrEmpty(CreateUserText) || !string.IsNullOrEmpty(CreateUserIconUrl);

		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }


		private LoginModel Model { get; set; }

		private RequiredFieldValidator<string> UsernameValidator { get; set; }

		private RequiredFieldValidator<string> PasswordValidator { get; set; }

		private bool ShowFailureText { get; set; }

		private bool UserAuthenticated { get; set; }

		private async Task ValidSubmit()
		{
			var loginCancelEventArgs = new LoginCancelEventArgs() { Sender = this };
			await OnLoggingIn.InvokeAsync(loginCancelEventArgs);

			if (loginCancelEventArgs.Cancel)
			{

				Model.Password = string.Empty;

			}
			else
			{

				var authenticateEventArgs = new AuthenticateEventArgs() { Sender = this };
				await OnAuthenticate.InvokeAsync(authenticateEventArgs);

				if (authenticateEventArgs.Authenticated)
				{

					await OnLoggedIn.InvokeAsync(EventArgs.Empty);

					if (!string.IsNullOrEmpty(DestinationPageUrl))
					{

						NavigationManager.NavigateTo(DestinationPageUrl);

					}

					ShowFailureText = false;

				}
				else
				{
					await OnLoginError.InvokeAsync(EventArgs.Empty);

					Model.Password = string.Empty;
					ShowFailureText = true;

				}

			}

		}

		protected override void HandleUnknownAttributes()
		{

			if (AdditionalAttributes?.Count > 0)
			{

				FailureTextStyle.FromUnknownAttributes(AdditionalAttributes, "FailureTextStyle-");
				TitleTextStyle.FromUnknownAttributes(AdditionalAttributes, "TitleTextStyle-");
				LabelStyle.FromUnknownAttributes(AdditionalAttributes, "LabelStyle-");
				CheckBoxStyle.FromUnknownAttributes(AdditionalAttributes, "CheckBoxStyle-");
				HyperLinkStyle.FromUnknownAttributes(AdditionalAttributes, "HyperLinkStyle-");
				InstructionTextStyle.FromUnknownAttributes(AdditionalAttributes, "InstructionTextStyle-");
				TextBoxStyle.FromUnknownAttributes(AdditionalAttributes, "TextBoxStyle-");
				LoginButtonStyle.FromUnknownAttributes(AdditionalAttributes, "LoginButtonStyle-");
				ValidatorTextStyle.FromUnknownAttributes(AdditionalAttributes, "ValidatorTextStyle-");

			}

			this.SetFontsFromAttributes(AdditionalAttributes);
			base.HandleUnknownAttributes();
		}

		protected override async Task OnInitializedAsync()
		{

			Model = new LoginModel
			{
				Username = UserName,
				Password = Password,
				RememberMe = RememberMeSet
			};

			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

			UserAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;

			await base.OnInitializedAsync();

		}

		protected override Task OnAfterRenderAsync(bool firstRender)
		{

			if (firstRender)
			{

				ValidatorTextStyle.CopyTo(UsernameValidator);
				ValidatorTextStyle.CopyTo(PasswordValidator);

			}

			return base.OnAfterRenderAsync(firstRender);

		}

	}

}
