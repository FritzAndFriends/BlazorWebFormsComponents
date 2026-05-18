using System.Collections.Generic;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Shim for System.Web.UI.WebControls.Panel — renders a &lt;div&gt; container.
	/// Child controls can be added to the Controls collection and will be rendered inside the div.
	/// </summary>
	public class Panel : WebControl
	{
		/// <summary>
		/// Gets the HTML tag for this control (div).
		/// </summary>
		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

		/// <summary>
		/// Gets the collection of child controls within this panel.
		/// </summary>
		public new List<WebControl> Controls { get; } = new();

		/// <summary>
		/// Renders the child controls inside the div.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			foreach (var child in Controls)
			{
				child.RenderControl(writer);
			}
		}
	}

	/// <summary>
	/// Shim for System.Web.UI.WebControls.PlaceHolder — invisible container that
	/// renders only its children without any wrapper element.
	/// </summary>
	public class PlaceHolder : WebControl
	{
		/// <summary>
		/// Gets the collection of child controls.
		/// </summary>
		public new List<WebControl> Controls { get; } = new();

		/// <summary>
		/// Renders only child controls — no outer tag.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			foreach (var child in Controls)
			{
				child.RenderControl(writer);
			}
		}
	}

	/// <summary>
	/// Shim for System.Web.UI.HtmlControls.HtmlGenericControl — renders any
	/// specified HTML tag with attributes and child content.
	/// </summary>
	public class HtmlGenericControl : WebControl
	{
		private readonly string _tagName;

		/// <summary>
		/// Creates an HtmlGenericControl with the specified tag name.
		/// </summary>
		/// <param name="tag">The HTML tag to render (e.g., "span", "div", "section").</param>
		public HtmlGenericControl(string tag = "span")
		{
			_tagName = tag;
		}

		/// <summary>
		/// Gets the collection of child controls.
		/// </summary>
		public new List<WebControl> Controls { get; } = new();

		/// <summary>
		/// Renders the specified tag with attributes and child content.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			AddAttributesToRender(writer);
			writer.RenderBeginTag(_tagName);
			foreach (var child in Controls)
			{
				child.RenderControl(writer);
			}
			RenderContents(writer);
			writer.RenderEndTag();
		}
	}

	/// <summary>
	/// Marker interface for naming container support. Controls implementing this
	/// interface create a naming scope for child control IDs, matching the
	/// System.Web.UI.INamingContainer interface from Web Forms.
	/// </summary>
	public interface INamingContainer { }
}
