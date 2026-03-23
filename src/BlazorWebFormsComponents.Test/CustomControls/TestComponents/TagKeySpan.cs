using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class TagKeySpan : WebControl
	{
		[Parameter]
		public string Text { get; set; }

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.Write(Text);
		}
	}
}
