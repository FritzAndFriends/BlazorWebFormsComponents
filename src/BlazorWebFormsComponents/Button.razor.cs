using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace BlazorWebFormsComponents {

	public partial class Button : BaseWebFormsComponent {

		[Parameter]
		public EventHandler<CommandEventArgs> OnCommand { get; set; }

		[Parameter]
		public string CommandName { get; set; }

		[Parameter]
		public object CommandArgument { get; set; }

		[Parameter]
		public EventCallback<MouseEventArgs> OnClick { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected void Click(MouseEventArgs args) {

			Console.WriteLine($"OnCommand: {OnCommand.GetType()}");
			if (OnCommand != null) {
				OnCommand.Invoke(null, new CommandEventArgs(CommandName, CommandArgument));
			} else {
				OnClick.InvokeAsync(args);
			}

		}

	}

}
