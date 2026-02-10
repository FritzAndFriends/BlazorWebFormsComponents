using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SimpleButton : WebControl
	{
		[Parameter]
		public string Text { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Button);
			writer.Write(Text);
			writer.RenderEndTag();
		}
	}
}
