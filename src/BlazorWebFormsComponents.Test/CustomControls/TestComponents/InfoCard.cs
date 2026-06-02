using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	/// <summary>
	/// A more complex custom WebControl that mimics a card/panel component
	/// typically built in Web Forms. Uses RenderContents to build nested HTML
	/// with a header and body section.
	/// 
	/// Original Web Forms code:
	/// <code>
	/// public class InfoCard : System.Web.UI.WebControls.WebControl
	/// {
	///     public string Title { get; set; }
	///     public string Body { get; set; }
	///     public string HeaderCssClass { get; set; } = "card-header";
	///     
	///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;
	///     
	///     protected override void RenderContents(HtmlTextWriter writer)
	///     {
	///         // Header
	///         writer.AddAttribute(HtmlTextWriterAttribute.Class, HeaderCssClass);
	///         writer.RenderBeginTag(HtmlTextWriterTag.Div);
	///         writer.RenderBeginTag(HtmlTextWriterTag.H3);
	///         writer.Write(Title);
	///         writer.RenderEndTag(); // h3
	///         writer.RenderEndTag(); // header div
	///         
	///         // Body
	///         writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-body");
	///         writer.RenderBeginTag(HtmlTextWriterTag.Div);
	///         writer.Write(Body);
	///         writer.RenderEndTag(); // body div
	///     }
	/// }
	/// </code>
	/// </summary>
	public class InfoCard : WebControl
	{
		[Parameter]
		public string Title { get; set; }

		[Parameter]
		public string Body { get; set; }

		[Parameter]
		public string HeaderCssClass { get; set; } = "card-header";

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			// Header
			writer.AddAttribute(HtmlTextWriterAttribute.Class, HeaderCssClass);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.RenderBeginTag(HtmlTextWriterTag.H3);
			writer.Write(Title);
			writer.RenderEndTag(); // h3
			writer.RenderEndTag(); // header div

			// Body
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-body");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			writer.Write(Body);
			writer.RenderEndTag(); // body div
		}
	}
}
