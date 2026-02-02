using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Provides a base class for custom controls that use HtmlTextWriter for rendering.
	/// This class allows Web Forms custom controls to be migrated to Blazor by providing
	/// a similar API surface to System.Web.UI.WebControls.WebControl.
	/// Base attributes (ID, CssClass, Style) are automatically added to the HtmlTextWriter
	/// before the Render method is called.
	/// </summary>
	/// <example>
	/// <code>
	/// public class HelloLabel : WebControl
	/// {
	///     [Parameter]
	///     public string Text { get; set; }
	///     
	///     [Parameter]
	///     public string Prefix { get; set; }
	///     
	///     protected override void Render(HtmlTextWriter writer)
	///     {
	///         writer.RenderBeginTag(HtmlTextWriterTag.Span);
	///         writer.Write($"{Prefix} {Text}");
	///         writer.RenderEndTag();
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class WebControl : BaseStyledComponent
	{
		/// <summary>
		/// Renders the control using the provided HtmlTextWriter.
		/// Override this method to provide custom rendering logic for your control.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		protected virtual void Render(HtmlTextWriter writer)
		{
			// Default implementation writes nothing
		}

		/// <summary>
		/// Renders the contents of the control using the provided HtmlTextWriter.
		/// Override this method if you only want to customize the inner content while
		/// maintaining the default outer tag rendering.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		protected virtual void RenderContents(HtmlTextWriter writer)
		{
			// Default implementation writes nothing
		}

		/// <summary>
		/// Public method to render the control to the provided HtmlTextWriter.
		/// This is used by composite controls to render child controls.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		internal void RenderControl(HtmlTextWriter writer)
		{
			Render(writer);
		}

		/// <summary>
		/// Adds the base component attributes (ID, CssClass, Style) to the HtmlTextWriter.
		/// This method is called automatically before Render(). 
		/// Derived classes should not need to call this method directly.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to add attributes to.</param>
		private void AddBaseAttributes(HtmlTextWriter writer)
		{
			// Apply base styles if they exist
			if (!string.IsNullOrEmpty(Style))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Style, Style);
			}

			if (!string.IsNullOrEmpty(CssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
			}

			if (!string.IsNullOrEmpty(ID))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
			}
		}

		/// <summary>
		/// Builds the render tree for the Blazor component by calling the Render method
		/// and converting the HtmlTextWriter output to a RenderFragment.
		/// </summary>
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			if (!Visible)
				return;

			using (var writer = new HtmlTextWriter())
			{
				// Automatically add base attributes before calling user's Render method
				AddBaseAttributes(writer);

				// Call the custom render method
				Render(writer);

				// Get the rendered HTML
				var html = writer.GetHtml();

				// Add the HTML to the render tree
				builder.AddMarkupContent(0, html);
			}
		}
	}
}
