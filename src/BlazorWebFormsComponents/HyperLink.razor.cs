using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class HyperLink : BaseWebFormsComponent
	{
		[Parameter]
		public string NavigationUrl { get; set; }

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public string Text { get; set; }
	}
}
