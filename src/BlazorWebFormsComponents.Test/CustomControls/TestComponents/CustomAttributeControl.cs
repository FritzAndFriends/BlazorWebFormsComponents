using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class CustomAttributeControl : WebControl
	{
		[Parameter]
		public string DataValue { get; set; }

		[Parameter]
		public string Text { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			if (!string.IsNullOrEmpty(DataValue))
				writer.AddAttribute("data-value", DataValue);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.Write(Text);
		}
	}
}
