using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorWebFormsComponents.Validations;

namespace BlazorWebFormsComponents
{
	public abstract class ButtonBaseComponent : BaseStyledComponent, IButtonComponent
	{

		[Parameter]
		public bool CausesValidation { get; set; } = true;

		[Parameter]
		public string ValidationGroup { get; set; }

		[CascadingParameter(Name = "ValidationGroupCoordinator")]
		protected ValidationGroupCoordinator Coordinator { get; set; }

		[Parameter]
		public string CommandName { get; set; }

		[Parameter]
		public object CommandArgument { get; set; }

		[Parameter]
		public virtual string PostBackUrl { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string OnClientClick { get; set; }

		[Parameter]
		public EventCallback<MouseEventArgs> OnClick { get; set; }

		[Parameter]
		public EventCallback<CommandEventArgs> OnCommand { get; set; }

		protected void Click()
		{
			// Trigger validation for the specific ValidationGroup if CausesValidation is true
			// Note: In Blazor, validation state is managed by EditContext. The OnClick event
			// will still fire regardless of validation state. The EditContext determines whether
			// OnValidSubmit or OnInvalidSubmit fires based on validation results.
			if (CausesValidation && Coordinator != null)
			{
				Coordinator.ValidateGroup(ValidationGroup);
			}

			if (!string.IsNullOrEmpty(CommandName))
			{
				var args = new CommandEventArgs(CommandName, CommandArgument);
				OnCommand.InvokeAsync(args);
				OnBubbledEvent(this, args);
			}
			else
			{
				OnClick.InvokeAsync(new MouseEventArgs());
			}
		}
	}
}
