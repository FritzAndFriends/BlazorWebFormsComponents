using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class PanelComposite : CompositeControl
	{
		[Parameter]
		public string Title { get; set; }

		[Parameter]
		public string Body { get; set; }

		protected override void CreateChildControls()
		{
			var header = new LiteralControl { Text = $"<h3>{Title}</h3>" };
			var content = new LiteralControl { Text = Body };
			Controls.Add(header);
			Controls.Add(content);
		}
	}
}
