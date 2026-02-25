using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class FormGroup : CompositeControl
	{
		[Parameter]
		public string LabelText { get; set; }

		[Parameter]
		public string ButtonText { get; set; }

		protected override void CreateChildControls()
		{
			if (!string.IsNullOrEmpty(LabelText))
			{
				Controls.Add(new SimpleLabel { Text = LabelText });
			}

			if (!string.IsNullOrEmpty(ButtonText))
			{
				Controls.Add(new SimpleButton { Text = ButtonText });
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderChildren(writer);
			writer.RenderEndTag();
		}
	}
}
