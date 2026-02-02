using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class StyledDiv : WebControl
	{
		[Parameter]
		public string Content { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "red");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.Write(Content);
			writer.RenderEndTag();
		}
	}
}
