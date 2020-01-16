using System;
using System.Collections.Generic;
using System.Text;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class Login : BaseWebFormsComponent
	{
		[Parameter] public string LoginButtonImageUrl { get; set; }
		[Parameter] public string LoginButtonText { get; set; } = "Log In";
		[Parameter] public ButtonType LoginButtonType { get; set; } = ButtonType.Button;

		//[Parameter] public Orientation Orientation { get; set; }
		[Parameter] public string MembershipProvider { get; set; }
		[Parameter] public string PasswordLabelText { get; set; } = "Password:";
		[Parameter] public string PasswordRecoveryText { get; set; }
		[Parameter] public string PasswordRecoveryUrl { get; set; }
		[Parameter] public string PasswordRecoveryIconUrl { get; set; }
		[Parameter] public string PasswordRequiredErrorMessage { get; set; } = "Password is required.";
		[Parameter] public bool RememberMeSet { get; set; }
		[Parameter] public string RememberMeText { get; set; } = "Remember me next time.";
		[Parameter] public bool RenderOuterTable { get; set; } = true;
		//[Parameter] public LoginTextLayout TextLayout { get; set; }
		[Parameter] public string TitleText { get; set; } = "Log In";
		[Parameter] public string UserName { get; set; }
		[Parameter] public string UserNameLabelText { get; set; } = "User Name:";
		[Parameter] public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";
		[Parameter] public bool VisibleWhenLoggedIn { get; set; } = true;
		//[Parameter] public LoginFailureAction FailureAction { get; set; }
		[Parameter] public string FailureText { get; set; } = "Your login attempt was not successful. Please try again.";
		//[Parameter] public ITemplate LayoutTemplate { get; set; }
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

		[Parameter] public EventCallback<EventArgs> OnLoggingIn { get; set; }
		[Parameter] public EventCallback<EventArgs> OnLoginError { get; set; }
		[Parameter] public EventCallback<EventArgs> OnLoggedIn { get; set; }
		[Parameter] public EventCallback<EventArgs> OnAuthenticate { get; set; }

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

		private bool HasHelp => !string.IsNullOrEmpty(HelpPageText) || !string.IsNullOrEmpty(HelpPageIconUrl);
		private bool HasPasswordRevocery => !string.IsNullOrEmpty(PasswordRecoveryText) || !string.IsNullOrEmpty(PasswordRecoveryIconUrl);
		private bool HasCreateUser => !string.IsNullOrEmpty(CreateUserText) || !string.IsNullOrEmpty(CreateUserIconUrl);

		private LoginModel model;

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

			}

			base.HandleUnknownAttributes();
		}

		protected override void OnInitialized()
		{
			model = new LoginModel
			{
				Username = UserName,
				RememberMe = RememberMeSet
			};


			base.OnInitialized();
		}
	}
}
