using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Provides a base class for custom controls that use HtmlTextWriter for rendering.
	/// This class allows Web Forms custom controls to be migrated to Blazor by providing
	/// a similar API surface to System.Web.UI.WebControls.WebControl.
	/// Attributes (ID, CssClass, Style, ToolTip, Enabled) are automatically added via
	/// <see cref="AddAttributesToRender"/> before <see cref="Render"/> is called.
	/// </summary>
	/// <example>
	/// Pattern 1 — Override Render for full control:
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
	/// Pattern 2 — Override TagKey + RenderContents (Web Forms pipeline):
	/// <code>
	/// public class AlertDiv : WebControl
	/// {
	///     [Parameter]
	///     public string Message { get; set; }
	///     
	///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;
	///     
	///     protected override void RenderContents(HtmlTextWriter writer)
	///     {
	///         writer.Write(Message);
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class WebControl : BaseStyledComponent
	{
		/// <summary>
		/// Gets the HTML tag type for this control. The default is
		/// <see cref="HtmlTextWriterTag.Span"/>. Subclasses override this to change
		/// the outer tag (e.g., <c>HtmlTextWriterTag.Div</c>).
		/// </summary>
		protected virtual HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

		/// <summary>
		/// Gets the string tag name derived from <see cref="TagKey"/>.
		/// </summary>
		public virtual string TagName => ResolveTagName(TagKey);

		/// <summary>
		/// Renders the control using the provided HtmlTextWriter.
		/// The default implementation calls <see cref="RenderBeginTag"/>,
		/// <see cref="RenderContents"/>, and <see cref="RenderEndTag"/> to produce
		/// the Web Forms rendering pipeline. Override this method to take full
		/// control of the rendered output.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		protected virtual void Render(HtmlTextWriter writer)
		{
			RenderBeginTag(writer);
			RenderContents(writer);
			RenderEndTag(writer);
		}

		/// <summary>
		/// Renders the contents of the control using the provided HtmlTextWriter.
		/// Override this method if you only want to customize the inner content while
		/// maintaining the default outer tag rendering via <see cref="TagKey"/>.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		protected virtual void RenderContents(HtmlTextWriter writer)
		{
			// Default implementation writes nothing
		}

		/// <summary>
		/// Renders the opening tag for the control using <see cref="TagKey"/>.
		/// This method does not call <see cref="AddAttributesToRender"/> —
		/// that is done by <see cref="BuildRenderTree"/> before <see cref="Render"/>.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		public virtual void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(TagKey);
		}

		/// <summary>
		/// Renders the closing tag for the control.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		public virtual void RenderEndTag(HtmlTextWriter writer)
		{
			writer.RenderEndTag();
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
		/// Adds the base component attributes (ID, CssClass, Style, ToolTip, Enabled)
		/// to the HtmlTextWriter. This method is called automatically by
		/// <see cref="BuildRenderTree"/> before <see cref="Render"/>.
		/// Override this method to add additional attributes before rendering.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to add attributes to.</param>
		protected virtual void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (!string.IsNullOrEmpty(ID))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
			}

			if (!string.IsNullOrEmpty(CssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
			}

			if (!string.IsNullOrEmpty(Style))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Style, Style);
			}

			if (!string.IsNullOrEmpty(ToolTip))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);
			}

			if (!Enabled)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
			}
		}

		/// <summary>
		/// Builds the render tree for the Blazor component by calling
		/// <see cref="AddAttributesToRender"/> and <see cref="Render"/>,
		/// then converting the HtmlTextWriter output to a RenderFragment.
		/// </summary>
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			if (!Visible)
				return;

			using (var writer = new HtmlTextWriter())
			{
				AddAttributesToRender(writer);
				Render(writer);
				builder.AddMarkupContent(0, writer.GetHtml());
			}
		}

		/// <summary>
		/// Maps an <see cref="HtmlTextWriterTag"/> enum value to its HTML tag name string.
		/// </summary>
		private static string ResolveTagName(HtmlTextWriterTag tag)
		{
			return tag switch
			{
				HtmlTextWriterTag.A => "a",
				HtmlTextWriterTag.Button => "button",
				HtmlTextWriterTag.Div => "div",
				HtmlTextWriterTag.Span => "span",
				HtmlTextWriterTag.Input => "input",
				HtmlTextWriterTag.Label => "label",
				HtmlTextWriterTag.P => "p",
				HtmlTextWriterTag.Table => "table",
				HtmlTextWriterTag.Tr => "tr",
				HtmlTextWriterTag.Td => "td",
				HtmlTextWriterTag.Th => "th",
				HtmlTextWriterTag.Tbody => "tbody",
				HtmlTextWriterTag.Thead => "thead",
				HtmlTextWriterTag.Ul => "ul",
				HtmlTextWriterTag.Li => "li",
				HtmlTextWriterTag.Select => "select",
				HtmlTextWriterTag.Option => "option",
				HtmlTextWriterTag.Img => "img",
				HtmlTextWriterTag.H1 => "h1",
				HtmlTextWriterTag.H2 => "h2",
				HtmlTextWriterTag.H3 => "h3",
				HtmlTextWriterTag.H4 => "h4",
				HtmlTextWriterTag.H5 => "h5",
				HtmlTextWriterTag.H6 => "h6",
				HtmlTextWriterTag.Form => "form",
				_ => tag.ToString().ToLowerInvariant()
			};
		}
	}
}
