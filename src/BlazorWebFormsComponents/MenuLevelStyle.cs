using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Style class for per-level menu item styling.
	/// Used in LevelMenuItemStyles, LevelSelectedStyles, and LevelSubMenuStyles collections.
	/// </summary>
	public class MenuLevelStyle : IStyle
	{
		public WebColor BackColor { get; set; }
		public WebColor BorderColor { get; set; }
		public BorderStyle BorderStyle { get; set; }
		public Unit BorderWidth { get; set; }
		public string CssClass { get; set; }
		public FontInfo Font { get; set; } = new FontInfo();
		public WebColor ForeColor { get; set; }
		public Unit Height { get; set; }
		public Unit Width { get; set; }

		/// <summary>
		/// Returns the CSS style string for this level style.
		/// </summary>
		public override string ToString() => this.ToStyle().NullIfEmpty();
	}
}
