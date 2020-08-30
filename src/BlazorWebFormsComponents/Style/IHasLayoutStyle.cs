using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public interface IHasLayoutStyle
	{

		// Cheer 1042 codingbandit 07/1/20 
		// Cheer 100 ramblinggeek 07/1/20 


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
