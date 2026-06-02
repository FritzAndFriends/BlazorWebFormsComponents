using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	/// <summary>
	/// A realistic custom WebControl that mimics what developers would have written
	/// in Web Forms: overrides TagKey for the outer element, uses RenderContents
	/// for the inner HTML, and AddAttributesToRender for custom attributes.
	/// 
	/// Original Web Forms code would have been:
	/// <code>
	/// public class StatusBadge : System.Web.UI.WebControls.WebControl
	/// {
	///     public string Status { get; set; }
	///     public string Label { get; set; }
	///     
	///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;
	///     
	///     protected override void AddAttributesToRender(HtmlTextWriter writer)
	///     {
	///         base.AddAttributesToRender(writer);
	///         writer.AddAttribute("data-status", Status?.ToLowerInvariant() ?? "unknown");
	///     }
	///     
	///     protected override void RenderContents(HtmlTextWriter writer)
	///     {
	///         writer.RenderBeginTag(HtmlTextWriterTag.Strong);
	///         writer.Write(Label ?? Status);
	///         writer.RenderEndTag();
	///     }
	/// }
	/// </code>
	/// 
	/// In Blazor, the ONLY change is the using/namespace — the code is identical.
	/// </summary>
	public class StatusBadge : WebControl
	{
		[Parameter]
		public string Status { get; set; }

		[Parameter]
		public string Label { get; set; }

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddAttribute("data-status", Status?.ToLowerInvariant() ?? "unknown");
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Strong);
			writer.Write(Label ?? Status);
			writer.RenderEndTag();
		}
	}
}
