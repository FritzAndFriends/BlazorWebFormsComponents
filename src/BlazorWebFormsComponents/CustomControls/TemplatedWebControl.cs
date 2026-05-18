using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Provides a base class for custom controls that combine HtmlTextWriter rendering
	/// with RenderFragment template regions. This is the Blazor equivalent of Web Forms
	/// controls that use ITemplate properties (HeaderTemplate, ContentTemplate, etc.).
	/// </summary>
	/// <remarks>
	/// In RenderContents (or Render), call <see cref="RenderTemplate"/> to insert a
	/// RenderFragment into the HtmlTextWriter output stream. The base class handles
	/// splitting the output and interleaving markup with Blazor render tree content.
	/// </remarks>
	/// <example>
	/// <code>
	/// public class SectionPanel : TemplatedWebControl
	/// {
	///     [Parameter] public RenderFragment HeaderTemplate { get; set; }
	///     [Parameter] public RenderFragment ContentTemplate { get; set; }
	///     
	///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;
	///     
	///     protected override void RenderContents(HtmlTextWriter writer)
	///     {
	///         writer.AddAttribute(HtmlTextWriterAttribute.Class, "header");
	///         writer.RenderBeginTag(HtmlTextWriterTag.Div);
	///         RenderTemplate(writer, HeaderTemplate);
	///         writer.RenderEndTag();
	///         
	///         writer.AddAttribute(HtmlTextWriterAttribute.Class, "content");
	///         writer.RenderBeginTag(HtmlTextWriterTag.Div);
	///         RenderTemplate(writer, ContentTemplate);
	///         writer.RenderEndTag();
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class TemplatedWebControl : WebControl
	{
		private const string PlaceholderPrefix = "<!--BWFC_TPL_";
		private const string PlaceholderSuffix = "-->";

		private readonly List<RenderFragment> _templateSlots = new();

		/// <summary>
		/// Captures any implicit content between named render fragment parameters.
		/// This prevents Razor-generated whitespace from leaking into the rendered output.
		/// Derived classes should use named RenderFragment parameters instead of ChildContent.
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Inserts a RenderFragment template into the HtmlTextWriter output at the current position.
		/// Call this from RenderContents or Render to place template content within the
		/// HtmlTextWriter-generated structure.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter being used for rendering.</param>
		/// <param name="template">The RenderFragment to insert. If null, nothing is written.</param>
		protected void RenderTemplate(HtmlTextWriter writer, RenderFragment template)
		{
			if (template == null) return;

			var index = _templateSlots.Count;
			_templateSlots.Add(template);
			writer.Write($"{PlaceholderPrefix}{index}{PlaceholderSuffix}");
		}

		/// <summary>
		/// Builds the render tree by calling the Render pipeline, then splitting the output
		/// on template placeholders and interleaving markup content with RenderFragment content.
		/// </summary>
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			if (!Visible) return;

			_templateSlots.Clear();

			using (var writer = new HtmlTextWriter())
			{
				AddAttributesToRender(writer);
				Render(writer);
				var html = writer.GetHtml();

				if (_templateSlots.Count == 0)
				{
					// No templates used — render as plain markup
					builder.AddMarkupContent(0, html);
				}
				else
				{
					// Split on placeholders and interleave
					var sequence = 0;
					var remaining = html;

					for (var i = 0; i < _templateSlots.Count; i++)
					{
						var placeholder = $"{PlaceholderPrefix}{i}{PlaceholderSuffix}";
						var placeholderIndex = remaining.IndexOf(placeholder, StringComparison.Ordinal);

						if (placeholderIndex >= 0)
						{
							// Emit markup before the placeholder
							var before = remaining.Substring(0, placeholderIndex);
							if (!string.IsNullOrEmpty(before))
							{
								builder.AddMarkupContent(sequence++, before);
							}

							// Emit the RenderFragment
							builder.AddContent(sequence++, _templateSlots[i]);

							// Advance past the placeholder
							remaining = remaining.Substring(placeholderIndex + placeholder.Length);
						}
					}

					// Emit any remaining markup after the last placeholder
					if (!string.IsNullOrEmpty(remaining))
					{
						builder.AddMarkupContent(sequence, remaining);
					}
				}
			}
		}
	}
}
