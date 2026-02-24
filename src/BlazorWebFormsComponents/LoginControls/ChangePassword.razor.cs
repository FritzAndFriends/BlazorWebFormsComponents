using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class ChangePassword : BaseStyledComponent
	{
		#region Obsolete

		[Parameter, Obsolete("MembershipProvider not supported in Blazor")]
		public string MembershipProvider { get; set; }

		#endregion

		#region Button Properties

		[Parameter] public string CancelButtonImageUrl { get; set; }
		[Parameter] public string CancelButtonText { get; set; } = "Cancel";
		[Parameter] public ButtonType CancelButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string CancelDestinationPageUrl { get; set; }

		[Parameter] public string ChangePasswordButtonImageUrl { get; set; }
		[Parameter] public string ChangePasswordButtonText { get; set; } = "Change Password";
		[Parameter] public ButtonType ChangePasswordButtonType { get; set; } = ButtonType.Button;

		[Parameter] public string ContinueButtonImageUrl { get; set; }
		[Parameter] public string ContinueButtonText { get; set; } = "Continue";
		[Parameter] public ButtonType ContinueButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string ContinueDestinationPageUrl { get; set; }

		#endregion

		#region Text Properties

		[Parameter] public string ChangePasswordTitleText { get; set; } = "Change Your Password";
		[Parameter] public string ConfirmNewPasswordLabelText { get; set; } = "Confirm New Password:";
		[Parameter] public string ConfirmPasswordCompareErrorMessage { get; set; } = "The Confirm New Password must match the New Password entry.";
		[Parameter] public string ConfirmPasswordRequiredErrorMessage { get; set; } = "Confirm New Password is required.";
		[Parameter] public string ChangePasswordFailureText { get; set; } = "Password incorrect or New Password invalid.";
		[Parameter] public string InstructionText { get; set; }
		[Parameter] public string NewPasswordLabelText { get; set; } = "New Password:";
		[Parameter] public string NewPasswordRegularExpression { get; set; }
		[Parameter] public string NewPasswordRegularExpressionErrorMessage { get; set; }
		[Parameter] public string NewPasswordRequiredErrorMessage { get; set; } = "New Password is required.";
		[Parameter] public string PasswordHintText { get; set; }
		[Parameter] public string PasswordLabelText { get; set; } = "Password:";
		[Parameter] public string PasswordRequiredErrorMessage { get; set; } = "Password is required.";
		[Parameter] public string SuccessPageUrl { get; set; }
		[Parameter] public string SuccessText { get; set; } = "Your password has been changed!";
		[Parameter] public string SuccessTitleText { get; set; } = "Change Password Complete";

		#endregion

		#region User Properties

		[Parameter] public bool DisplayUserName { get; set; }
		[Parameter] public string UserName { get => Model?.UserName ?? string.Empty; set { if (Model != null) Model.UserName = value; } }
		[Parameter] public string UserNameLabelText { get; set; } = "User Name:";
		[Parameter] public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";

		#endregion

		#region Link Properties

		[Parameter] public string CreateUserIconUrl { get; set; }
		[Parameter] public string CreateUserText { get; set; }
		[Parameter] public string CreateUserUrl { get; set; }
		[Parameter] public string EditProfileIconUrl { get; set; }
		[Parameter] public string EditProfileText { get; set; }
		[Parameter] public string EditProfileUrl { get; set; }
		[Parameter] public string HelpPageIconUrl { get; set; }
		[Parameter] public string HelpPageText { get; set; }
		[Parameter] public string HelpPageUrl { get; set; }
		[Parameter] public string PasswordRecoveryIconUrl { get; set; }
		[Parameter] public string PasswordRecoveryText { get; set; }
		[Parameter] public string PasswordRecoveryUrl { get; set; }

		#endregion

		#region Layout Properties

		[Parameter] public int BorderPadding { get; set; } = 1;
		[Parameter] public bool RenderOuterTable { get; set; } = true;
		[Parameter] public Orientation Orientation { get; set; } = Orientation.Vertical;
		[Parameter] public LoginTextLayout TextLayout { get; set; } = LoginTextLayout.TextOnLeft;

		#endregion

		#region Events

		[Parameter] public EventCallback<EventArgs> OnCancelButtonClick { get; set; }
		[Parameter] public EventCallback<EventArgs> OnChangedPassword { get; set; }
		[Parameter] public EventCallback<EventArgs> OnChangePasswordError { get; set; }
		[Parameter] public EventCallback<LoginCancelEventArgs> OnChangingPassword { get; set; }
		[Parameter] public EventCallback<EventArgs> OnContinueButtonClick { get; set; }

		#endregion

		#region Templates

		[Parameter] public RenderFragment ChildContent { get; set; }
		[Parameter] public RenderFragment ChangePasswordTemplate { get; set; }
		[Parameter] public RenderFragment SuccessTemplate { get; set; }

		#endregion

		#region Style

		[CascadingParameter(Name = "FailureTextStyle")]
		private TableItemStyle FailureTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "TitleTextStyle")]
		private TableItemStyle TitleTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "LabelStyle")]
		private TableItemStyle LabelStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "InstructionTextStyle")]
		private TableItemStyle InstructionTextStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "TextBoxStyle")]
		private Style TextBoxStyle { get; set; } = new Style();

		[CascadingParameter(Name = "LoginButtonStyle")]
		private Style LoginButtonStyle { get; set; } = new Style();

		[CascadingParameter(Name = "ValidatorTextStyle")]
		private Style ValidatorTextStyle { get; set; } = new Style();

		[CascadingParameter(Name = "HyperLinkStyle")]
		private TableItemStyle HyperLinkStyle { get; set; } = new TableItemStyle();

		#endregion

		#region Services

		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		#endregion

		#region Internal State

		private ChangePasswordModel Model { get; set; }
		private bool ShowSuccessView { get; set; }
		private bool ShowFailureText { get; set; }

		private bool HasHelp => !string.IsNullOrEmpty(HelpPageText) || !string.IsNullOrEmpty(HelpPageIconUrl);
		private bool HasPasswordRecovery => !string.IsNullOrEmpty(PasswordRecoveryText) || !string.IsNullOrEmpty(PasswordRecoveryIconUrl);
		private bool HasCreateUser => !string.IsNullOrEmpty(CreateUserText) || !string.IsNullOrEmpty(CreateUserIconUrl);
		private bool HasEditProfile => !string.IsNullOrEmpty(EditProfileText) || !string.IsNullOrEmpty(EditProfileIconUrl);

		public string CurrentPassword { get => Model?.CurrentPassword ?? string.Empty; set { if (Model != null) Model.CurrentPassword = value; } }
		public string NewPassword { get => Model?.NewPassword ?? string.Empty; set { if (Model != null) Model.NewPassword = value; } }

		#endregion

		protected override async Task OnInitializedAsync()
		{
			Model = new ChangePasswordModel();

			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			if (!DisplayUserName && authState.User?.Identity?.IsAuthenticated == true)
			{
				Model.UserName = authState.User.Identity.Name ?? string.Empty;
			}

			await base.OnInitializedAsync();
		}

		private async Task HandleValidSubmit()
		{
			var cancelArgs = new LoginCancelEventArgs { Sender = this };
			await OnChangingPassword.InvokeAsync(cancelArgs);

			if (cancelArgs.Cancel)
			{
				Model.CurrentPassword = string.Empty;
				Model.NewPassword = string.Empty;
				Model.ConfirmNewPassword = string.Empty;
				return;
			}

			// The developer must handle OnChangingPassword to do the actual password change.
			// If they didn't cancel, we consider it successful.
			ShowFailureText = false;
			ShowSuccessView = true;
			await OnChangedPassword.InvokeAsync(EventArgs.Empty);
		}

		internal async Task HandleChangePasswordError()
		{
			ShowFailureText = true;
			Model.CurrentPassword = string.Empty;
			Model.NewPassword = string.Empty;
			Model.ConfirmNewPassword = string.Empty;
			await OnChangePasswordError.InvokeAsync(EventArgs.Empty);
		}

		private async Task HandleCancelClick()
		{
			await OnCancelButtonClick.InvokeAsync(EventArgs.Empty);
			if (!string.IsNullOrEmpty(CancelDestinationPageUrl))
			{
				NavigationManager.NavigateTo(CancelDestinationPageUrl);
			}
		}

		private async Task HandleContinueClick()
		{
			await OnContinueButtonClick.InvokeAsync(EventArgs.Empty);
			if (!string.IsNullOrEmpty(ContinueDestinationPageUrl))
			{
				NavigationManager.NavigateTo(ContinueDestinationPageUrl);
			}
			else if (!string.IsNullOrEmpty(SuccessPageUrl))
			{
				NavigationManager.NavigateTo(SuccessPageUrl);
			}
		}

		protected override void HandleUnknownAttributes()
		{
			if (AdditionalAttributes?.Count > 0)
			{
				FailureTextStyle.FromUnknownAttributes(AdditionalAttributes, "FailureTextStyle-");
				TitleTextStyle.FromUnknownAttributes(AdditionalAttributes, "TitleTextStyle-");
				LabelStyle.FromUnknownAttributes(AdditionalAttributes, "LabelStyle-");
				InstructionTextStyle.FromUnknownAttributes(AdditionalAttributes, "InstructionTextStyle-");
				TextBoxStyle.FromUnknownAttributes(AdditionalAttributes, "TextBoxStyle-");
				LoginButtonStyle.FromUnknownAttributes(AdditionalAttributes, "LoginButtonStyle-");
				ValidatorTextStyle.FromUnknownAttributes(AdditionalAttributes, "ValidatorTextStyle-");
				HyperLinkStyle.FromUnknownAttributes(AdditionalAttributes, "HyperLinkStyle-");
			}

			this.SetFontsFromAttributes(AdditionalAttributes);
			base.HandleUnknownAttributes();
		}

		public class ChangePasswordModel
		{
			public string UserName { get; set; } = string.Empty;
			public string CurrentPassword { get; set; } = string.Empty;
			public string NewPassword { get; set; } = string.Empty;
			public string ConfirmNewPassword { get; set; } = string.Empty;
		}
	}
}
