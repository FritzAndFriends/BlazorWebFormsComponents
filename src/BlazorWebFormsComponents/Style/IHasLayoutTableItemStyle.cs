using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public interface IHasLayoutTableItemStyle : IHasLayoutStyle
	{
		HorizontalAlign HorizontalAlign { get; set; }

		VerticalAlign VerticalAlign { get; set; }

		bool Wrap { get; set; }
	}

}
