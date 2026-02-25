using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class PasswordRecovery : BaseStyledComponent
	{
		#region Obsolete

		[Parameter, Obsolete("MembershipProvider not supported in Blazor")]
		public string MembershipProvider { get; set; }

		#endregion

		#region UserName Step Properties

		[Parameter] public string UserNameLabelText { get; set; } = "User Name:";
		[Parameter] public string UserNameTitleText { get; set; } = "Forgot Your Password?";
		[Parameter] public string UserNameInstructionText { get; set; } = "Enter your User Name to receive your password.";
		[Parameter] public string UserNameFailureText { get; set; } = "Your attempt to retrieve your password was not successful. Please try again.";
		[Parameter] public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";
		[Parameter] public string SubmitButtonText { get; set; } = "Submit";
		[Parameter] public ButtonType SubmitButtonType { get; set; } = ButtonType.Button;
		[Parameter] public string SubmitButtonImageUrl { get; set; }

		#endregion

		#region Question Step Properties

		[Parameter] public string QuestionLabelText { get; set; } = "Answer:";
		[Parameter] public string QuestionTitleText { get; set; } = "Identity Confirmation";
		[Parameter] public string QuestionInstructionText { get; set; } = "Answer the following question to receive your password.";
		[Parameter] public string QuestionFailureText { get; set; } = "Your answer could not be verified. Please try again.";
		[Parameter] public string AnswerRequiredErrorMessage { get; set; } = "Answer is required.";

		#endregion

		#region Success Step Properties

		[Parameter] public string SuccessText { get; set; } = "Your password has been sent to you.";
		[Parameter] public string SuccessPageUrl { get; set; }

		#endregion

		#region General Properties

		[Parameter] public string GeneralFailureText { get; set; } = "Your attempt to retrieve your password was not successful. Please try again.";
		[Parameter] public string MailDefinition { get; set; }
		[Parameter] public string HelpPageIconUrl { get; set; }
		[Parameter] public string HelpPageText { get; set; }
		[Parameter] public string HelpPageUrl { get; set; }

		#endregion

		#region Layout Properties

		[Parameter] public int BorderPadding { get; set; } = 1;
		[Parameter] public bool RenderOuterTable { get; set; } = true;

		#endregion

		#region Events

		[Parameter] public EventCallback<LoginCancelEventArgs> OnVerifyingUser { get; set; }
		[Parameter] public EventCallback<EventArgs> OnUserLookupError { get; set; }
		[Parameter] public EventCallback<LoginCancelEventArgs> OnVerifyingAnswer { get; set; }
		[Parameter] public EventCallback<EventArgs> OnAnswerLookupError { get; set; }
		[Parameter] public EventCallback<MailMessageEventArgs> OnSendingMail { get; set; }
		[Parameter] public EventCallback<SendMailErrorEventArgs> OnSendMailError { get; set; }

		#endregion

		#region Templates

		[Parameter] public RenderFragment ChildContent { get; set; }
		[Parameter] public RenderFragment UserNameTemplate { get; set; }
		[Parameter] public RenderFragment QuestionTemplate { get; set; }
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
		private Style SubmitButtonStyle { get; set; } = new Style();

		[CascadingParameter(Name = "ValidatorTextStyle")]
		private Style ValidatorTextStyle { get; set; } = new Style();

		[CascadingParameter(Name = "HyperLinkStyle")]
		private TableItemStyle HyperLinkStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "SuccessTextStyle")]
		private TableItemStyle SuccessTextStyle { get; set; } = new TableItemStyle();

		#endregion

		#region Services

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		#endregion

		#region Internal State

		private PasswordRecoveryModel Model { get; set; }

		/// <summary>
		/// 0 = UserName step, 1 = Question step, 2 = Success step
		/// </summary>
		private int CurrentStep { get; set; }

		private bool ShowFailureText { get; set; }
		private string CurrentFailureText { get; set; }
		private string QuestionText { get; set; }

		/// <summary>
		/// The username entered in Step 1, displayed read-only in Step 2
		/// </summary>
		public string UserName { get => Model?.UserName ?? string.Empty; set { if (Model != null) Model.UserName = value; } }

		/// <summary>
		/// The answer entered in Step 2
		/// </summary>
		public string Answer { get => Model?.Answer ?? string.Empty; set { if (Model != null) Model.Answer = value; } }

		private bool HasHelp => !string.IsNullOrEmpty(HelpPageText) || !string.IsNullOrEmpty(HelpPageIconUrl);

		#endregion

		protected override async Task OnInitializedAsync()
		{
			Model = new PasswordRecoveryModel();
			await base.OnInitializedAsync();
		}

		private async Task HandleUserNameSubmit()
		{
			var cancelArgs = new LoginCancelEventArgs { Sender = this };
			await OnVerifyingUser.InvokeAsync(cancelArgs);

			if (cancelArgs.Cancel)
			{
				ShowFailureText = true;
				CurrentFailureText = UserNameFailureText;
				await OnUserLookupError.InvokeAsync(EventArgs.Empty);
				return;
			}

			// Developer handles OnVerifyingUser to validate the username.
			// If not cancelled, move to question step or success step.
			ShowFailureText = false;
			CurrentStep = 1;
		}

		private async Task HandleQuestionSubmit()
		{
			var cancelArgs = new LoginCancelEventArgs { Sender = this };
			await OnVerifyingAnswer.InvokeAsync(cancelArgs);

			if (cancelArgs.Cancel)
			{
				ShowFailureText = true;
				CurrentFailureText = QuestionFailureText;
				Model.Answer = string.Empty;
				await OnAnswerLookupError.InvokeAsync(EventArgs.Empty);
				return;
			}

			// Developer handles OnVerifyingAnswer to validate the answer.
			// If not cancelled, proceed to send mail and show success.
			ShowFailureText = false;

			var mailArgs = new MailMessageEventArgs { Sender = this };
			await OnSendingMail.InvokeAsync(mailArgs);

			CurrentStep = 2;

			if (!string.IsNullOrEmpty(SuccessPageUrl))
			{
				NavigationManager.NavigateTo(SuccessPageUrl);
			}
		}

		/// <summary>
		/// Call this from the OnVerifyingUser handler to set the security question text
		/// that will be displayed in Step 2.
		/// </summary>
		public void SetQuestion(string question)
		{
			QuestionText = question;
		}

		/// <summary>
		/// Call this to skip the question step and go directly to success
		/// (e.g., when no security question is configured).
		/// </summary>
		public async Task SkipToSuccess()
		{
			ShowFailureText = false;

			var mailArgs = new MailMessageEventArgs { Sender = this };
			await OnSendingMail.InvokeAsync(mailArgs);

			CurrentStep = 2;

			if (!string.IsNullOrEmpty(SuccessPageUrl))
			{
				NavigationManager.NavigateTo(SuccessPageUrl);
			}
		}

		internal async Task HandleSendMailError(string errorMessage)
		{
			await OnSendMailError.InvokeAsync(new SendMailErrorEventArgs
			{
				ErrorMessage = errorMessage,
				Sender = this
			});
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
				SubmitButtonStyle.FromUnknownAttributes(AdditionalAttributes, "SubmitButtonStyle-");
				ValidatorTextStyle.FromUnknownAttributes(AdditionalAttributes, "ValidatorTextStyle-");
				HyperLinkStyle.FromUnknownAttributes(AdditionalAttributes, "HyperLinkStyle-");
				SuccessTextStyle.FromUnknownAttributes(AdditionalAttributes, "SuccessTextStyle-");
			}

			base.HandleUnknownAttributes();
		}

		public class PasswordRecoveryModel
		{
			public string UserName { get; set; } = string.Empty;
			public string Answer { get; set; } = string.Empty;
		}
	}
}
