using BlazorWebFormsComponents.CustomControls;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SearchBox : CompositeControl
	{
		protected override void CreateChildControls()
		{
			var label = new SimpleLabel { Text = "Search:" };
			var button = new SimpleButton { Text = "Go" };

			Controls.Add(label);
			Controls.Add(button);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			AddBaseAttributes(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-box");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderChildren(writer);
			writer.RenderEndTag();
		}
	}
}
