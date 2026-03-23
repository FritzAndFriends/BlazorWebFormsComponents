using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class TagKeyTable : WebControl
	{
		[Parameter]
		public string CellContent { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Table;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.Write(CellContent);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
	}
}
