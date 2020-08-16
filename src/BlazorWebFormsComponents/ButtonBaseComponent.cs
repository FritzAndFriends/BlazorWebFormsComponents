using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorWebFormsComponents
{
	public abstract class ButtonBaseComponent : BaseWebFormsComponent, IButtonComponent
	{

		[Parameter]
		public bool CausesValidation { get; set; } = true;

		[Parameter]
		public string CommandName { get; set; }

		[Parameter]
		public object CommandArgument { get; set; }

		[Parameter]
		public string PostBackUrl { get; set; }

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
