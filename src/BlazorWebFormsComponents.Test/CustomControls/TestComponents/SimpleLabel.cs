using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SimpleLabel : WebControl
	{
		[Parameter]
		public string Text { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Label);
			writer.Write(Text);
			writer.RenderEndTag();
		}
	}
}
