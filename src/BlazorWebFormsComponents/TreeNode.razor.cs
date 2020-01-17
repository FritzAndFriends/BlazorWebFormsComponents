using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class TreeNode : BaseWebFormsComponent
	{

		[Parameter]
		public string NavigateUrl { get; set; }

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string Value { get; set; }

		[Parameter]
		public bool Expanded { get; set; }

	}
}
