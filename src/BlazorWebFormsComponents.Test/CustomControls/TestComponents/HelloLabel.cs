using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class HelloLabel : WebControl
	{
		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string Prefix { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write($"{Prefix} {Text}");
			writer.RenderEndTag();
		}
	}
}
