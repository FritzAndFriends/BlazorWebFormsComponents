using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class TagKeyDiv : WebControl
	{
		[Parameter]
		public string Content { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.Write(Content);
		}
	}
}
