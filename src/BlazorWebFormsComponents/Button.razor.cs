using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace BlazorWebFormsComponents {

	public partial class Button : BaseWebFormsComponent {

		[Parameter]
		public string OnClientClick { get; set; }

		[Parameter]
		public EventCallback<CommandEventArgs> OnCommand { get; set; }

		[Parameter]
		public string CommandName { get; set; }

		[Parameter]
		public object CommandArgument { get; set; }

		[Parameter]
		public EventCallback<MouseEventArgs> OnClick { get; set; }

		[Parameter]
		public string Text { get; set; }

		protected void Click() {

			if (OnCommand.HasDelegate) {

				var args = new CommandEventArgs(CommandName, CommandArgument);
				OnCommand.InvokeAsync(args);
				OnBubbledEvent(this, args);
			} else {
				OnClick.InvokeAsync(new MouseEventArgs());
			}

		}

	}

}
