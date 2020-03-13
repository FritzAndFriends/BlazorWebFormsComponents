using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace BlazorWebFormsComponents {

	public partial class Button : BaseWebFormsComponent {

		[Parameter]
		public EventCallback<CommandEventArgs> OnCommand { get; set; }

		[Parameter]
		public string CommandName { get; set; }

		[Parameter]
		public object CommandArgument { get; set; }

		[Parameter]
		public EventCallback<MouseEventArgs> OnClick { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected void Click() {

			if (OnCommand.HasDelegate) {
				OnCommand.InvokeAsync(new CommandEventArgs(CommandName, CommandArgument));
			} else {
				OnClick.InvokeAsync(new MouseEventArgs());
			}

		}

	}

}