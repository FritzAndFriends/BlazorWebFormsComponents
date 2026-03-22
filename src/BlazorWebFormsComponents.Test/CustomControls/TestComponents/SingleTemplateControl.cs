using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SingleTemplateControl : TemplatedWebControl
	{
		[Parameter]
		public RenderFragment ItemTemplate { get; set; }

		[Parameter]
		public string Title { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.H2);
			writer.Write(Title);
			writer.RenderEndTag();

			RenderTemplate(writer, ItemTemplate);
		}
	}
}
