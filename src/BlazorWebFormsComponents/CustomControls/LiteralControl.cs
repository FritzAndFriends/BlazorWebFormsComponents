using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Shim for System.Web.UI.LiteralControl — renders raw text/HTML content.
	/// This control does not render an outer tag; it only outputs its Text property.
	/// </summary>
	public class LiteralControl : WebControl
	{
		/// <summary>
		/// Gets or sets the text content to render.
		/// </summary>
		[Parameter]
		public string Text { get; set; } = string.Empty;

		/// <summary>
		/// Renders only the text content — no outer tag.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(Text);
		}
	}

	/// <summary>
	/// Alias for LiteralControl, matching System.Web.UI.WebControls.Literal.
	/// </summary>
	public class Literal : LiteralControl { }
}
