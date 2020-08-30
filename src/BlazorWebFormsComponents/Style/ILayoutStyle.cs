using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public interface ILayoutStyle
	{
		WebColor BackColor { get; set; }

		WebColor BorderColor { get; set; }

		BorderStyle BorderStyle { get; set; }

		Unit BorderWidth { get; set; }

		string CssClass { get; set; }

		WebColor ForeColor { get; set; }

		Unit Height { get; set; }

		Unit Width { get; set; }
	}
}
