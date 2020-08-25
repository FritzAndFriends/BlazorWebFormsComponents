using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorWebFormsComponents
{
	public interface IButtonComponent
	{
		public string CommandName { get; set; }

		public object CommandArgument { get; set; }

		public string OnClientClick { get; set; }

		public string PostBackUrl { get; set; }

		public string Text { get; set; }

		public EventCallback<MouseEventArgs> OnClick { get; set; }

		public EventCallback<CommandEventArgs> OnCommand { get; set; }
	}
}
