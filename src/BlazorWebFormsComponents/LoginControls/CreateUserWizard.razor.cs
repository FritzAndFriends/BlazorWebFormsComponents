using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class CreateUserWizard : BaseStyledComponent
	{
		#region Obsolete

		[Parameter, Obsolete("MembershipProvider not supported in Blazor")]
		public string MembershipProvider { get; set; }

		#endregion

		#region User Properties

		[Parameter] public string UserName { get => Model?.UserName ?? string.Empty; set { if (Model != null) Model.UserName = value; } }
		[Parameter] public string UserNameLabelText { get; set; } = "User Name:";
		[Parameter] public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";
		[Parameter] public string Password { get => Model?.Password ?? string.Empty; set { if (Model != null) Model.Password = value; } }
		[Parameter] public string PasswordLabelText { get; set; } = "Password:";
		[Parameter] public string PasswordRequiredErrorMessage { get; set; } = "Password is required.";
		[Parameter] public string PasswordHintText { get; set; }
		[Parameter] public string PasswordRegularExpression { get; set; }
		[Parameter] public string PasswordRegularExpressionErrorMessage { get; set; }
		[Parameter] public string ConfirmPasswordLabelText { get; set; } = "Confirm Password:";
		[Parameter] public string ConfirmPasswordCompareErrorMessage { get; set; } = "The Password and Confirmation Password must match.";
		[Parameter] public string ConfirmPasswordRequiredErrorMessage { get; set; } = "Confirm Password is required.";
		[Parameter] public string Email { get => Model?.Email ?? string.Empty; set { if (Model != null) Model.Email = value; } }
		[Parameter] public string EmailLabelText { get; set; } = "E-mail:";
		[Parameter] public string EmailRequiredErrorMessage { get; set; } = "E-mail is required.";
		[Parameter] public string EmailRegularExpression { get; set; }
		[Parameter] public string EmailRegularExpressionErrorMessage { get; set; }
		[Parameter] public string Question { get => Model?.Question ?? string.Empty; set { if (Model != null) Model.Question = value; } }
		[Parameter] public string QuestionLabelText { get; set; } = "Security Question:";
		[Parameter] public string QuestionRequiredErrorMessage { get; set; } = "Security question is required.";
		[Parameter] public string Answer { get => Model?.Answer ?? string.Empty; set { if (Model != null) Model.Answer = value; } }
		[Parameter] public string AnswerLabelText { get; set; } = "Security Answer:";
		[Parameter] public string AnswerRequiredErrorMessage { get; set; } = "Security answer is required.";
		[Parameter] public bool RequireEmail { get; set; } = true;
		[Parameter] public bool AutoGeneratePassword { get; set; }
		[Parameter] public bool DisableCreatedUser { get; set; }
		[Parameter] public bool LoginCreatedUser { get; set; } = true;

		#endregion

		#region Button Properties

		[Parameter] public string CancelButtonImageUrl { get; set; }
		[Parameter] public string CancelButtonText { get; set; } = "Cancel";
		[Parameter] public ButtonType CancelButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string CancelDestinationPageUrl { get; set; }
		[Parameter] public bool DisplayCancelButton { get; set; }

		[Parameter] public string CreateUserButtonImageUrl { get; set; }
		[Parameter] public string CreateUserButtonText { get; set; } = "Create User";
		[Parameter] public ButtonType CreateUserButtonType { get; set; } = ButtonType.Button;

		[Parameter] public string ContinueButtonImageUrl { get; set; }
		[Parameter] public string ContinueButtonText { get; set; } = "Continue";
		[Parameter] public ButtonType ContinueButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string ContinueDestinationPageUrl { get; set; }

		#endregion

		#region Text Properties

		[Parameter] public string CompleteSuccessText { get; set; } = "Your account has been successfully created.";
		[Parameter] public string InstructionText { get; set; }
		[Parameter] public string DuplicateEmailErrorMessage { get; set; } = "The e-mail address that you entered is already in use. Please enter a different e-mail address.";
		[Parameter] public string DuplicateUserNameErrorMessage { get; set; } = "Please enter a different user name.";
		[Parameter] public string InvalidAnswerErrorMessage { get; set; } = "Please enter a different security answer.";
		[Parameter] public string InvalidEmailErrorMessage { get; set; } = "Please enter a valid e-mail address.";
		[Parameter] public string InvalidPasswordErrorMessage { get; set; } = "Password length minimum: {0}. Non-alphanumeric characters required: {1}.";
		[Parameter] public string InvalidQuestionErrorMessage { get; set; } = "Please enter a different security question.";
		[Parameter] public string UnknownErrorMessage { get; set; } = "Your account was not created. Please try again.";

		#endregion

		#region Link Properties

		[Parameter] public string EditProfileIconUrl { get; set; }
		[Parameter] public string EditProfileText { get; set; }
		[Parameter] public string EditProfileUrl { get; set; }
		[Parameter] public string HelpPageIconUrl { get; set; }
		[Parameter] public string HelpPageText { get; set; }
		[Parameter] public string HelpPageUrl { get; set; }

		#endregion

		#region Layout Properties

		[Parameter] public int ActiveStepIndex { get; set; }
		[Parameter] public int BorderPadding { get; set; } = 1;
		[Parameter] public bool RenderOuterTable { get; set; } = true;
		[Parameter] public bool DisplaySideBar { get; set; } = true;

		#endregion

		#region Events

		[Parameter] public EventCallback<LoginCancelEventArgs> OnCreatingUser { get; set; }
		[Parameter] public EventCallback<EventArgs> OnCreatedUser { get; set; }
		[Parameter] public EventCallback<CreateUserErrorEventArgs> OnCreateUserError { get; set; }
		[Parameter] public EventCallback<EventArgs> OnCancelButtonClick { get; set; }
		[Parameter] public EventCallback<EventArgs> OnContinueButtonClick { get; set; }
		[Parameter] public EventCallback<EventArgs> OnActiveStepChanged { get; set; }
		[Parameter] public EventCallback<EventArgs> OnNextButtonClick { get; set; }
		[Parameter] public EventCallback<EventArgs> OnPreviousButtonClick { get; set; }
		[Parameter] public EventCallback<EventArgs> OnFinishButtonClick { get; set; }

		#endregion

		#region Templates

		[Parameter] public RenderFragment ChildContent { get; set; }
		[Parameter] public RenderFragment CreateUserStep { get; set; }
		[Parameter] public RenderFragment CompleteStep { get; set; }
		[Parameter] public RenderFragment SideBarTemplate { get; set; }
		[Parameter] public RenderFragment HeaderTemplate { get; set; }

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
		protected NavigationManager NavigationManager { get; set; }

		#endregion

		#region Internal State

		private CreateUserModel Model { get; set; }
		private bool ShowCompleteStep { get; set; }
		private bool ShowFailureText { get; set; }
		private string FailureText { get; set; }

		private bool HasHelp => !string.IsNullOrEmpty(HelpPageText) || !string.IsNullOrEmpty(HelpPageIconUrl);
		private bool HasEditProfile => !string.IsNullOrEmpty(EditProfileText) || !string.IsNullOrEmpty(EditProfileIconUrl);
		private bool ShowSecurityQuestion => !string.IsNullOrEmpty(Model?.Question);

		#endregion

		protected override async Task OnInitializedAsync()
		{
			Model = new CreateUserModel();
			await base.OnInitializedAsync();
		}

		private async Task HandleValidSubmit()
		{
			var cancelArgs = new LoginCancelEventArgs { Sender = this };
			await OnCreatingUser.InvokeAsync(cancelArgs);

			if (cancelArgs.Cancel)
			{
				return;
			}

			// Developer handles OnCreatingUser to perform actual user creation.
			// If not cancelled, we consider it successful.
			ShowFailureText = false;
			ShowCompleteStep = true;
			ActiveStepIndex = 1;
			await OnCreatedUser.InvokeAsync(EventArgs.Empty);
			await OnActiveStepChanged.InvokeAsync(EventArgs.Empty);
		}

		internal async Task HandleCreateUserError(string errorMessage)
		{
			ShowFailureText = true;
			FailureText = errorMessage ?? UnknownErrorMessage;
			await OnCreateUserError.InvokeAsync(new CreateUserErrorEventArgs
			{
				ErrorMessage = FailureText,
				Sender = this
			});
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

		public class CreateUserModel
		{
			public string UserName { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
			public string ConfirmPassword { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string Question { get; set; } = string.Empty;
			public string Answer { get; set; } = string.Empty;
		}
	}
}
