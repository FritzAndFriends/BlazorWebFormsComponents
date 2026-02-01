using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class CustomButton : WebControl
	{
		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string ButtonType { get; set; } = "button";

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Type, ButtonType);
			writer.RenderBeginTag(HtmlTextWriterTag.Button);
			writer.Write(Text);
			writer.RenderEndTag();
		}
	}
}
