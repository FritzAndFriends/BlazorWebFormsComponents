using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Represents a skin that can be applied to a control, holding visual property
	/// values that correspond to <see cref="BaseStyledComponent"/> parameters.
	/// Null properties indicate "not set" — the component's explicit value takes precedence
	/// (StyleSheetTheme semantics).
	/// </summary>
	public class ControlSkin
	{
		public WebColor BackColor { get; set; }

		public WebColor ForeColor { get; set; }

		public WebColor BorderColor { get; set; }

		public BorderStyle? BorderStyle { get; set; }

		public Unit? BorderWidth { get; set; }

		public string CssClass { get; set; }

		public Unit? Height { get; set; }

		public Unit? Width { get; set; }

		public FontInfo Font { get; set; }

		public string ToolTip { get; set; }
	}
}
