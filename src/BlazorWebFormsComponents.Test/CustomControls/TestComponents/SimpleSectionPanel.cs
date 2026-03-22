using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SimpleSectionPanel : TemplatedWebControl
	{
		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment ContentTemplate { get; set; }

		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "header");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderTemplate(writer, HeaderTemplate);
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "content");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderTemplate(writer, ContentTemplate);
			writer.RenderEndTag();

			if (FooterTemplate != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "footer");
				writer.RenderBeginTag(HtmlTextWriterTag.Div);
				RenderTemplate(writer, FooterTemplate);
				writer.RenderEndTag();
			}
		}
	}
}
