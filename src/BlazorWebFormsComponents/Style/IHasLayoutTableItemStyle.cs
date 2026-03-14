using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public interface IHasLayoutTableItemStyle : IHasLayoutStyle
	{
		EnumParameter<HorizontalAlign> HorizontalAlign { get; set; }

		EnumParameter<VerticalAlign> VerticalAlign { get; set; }

		bool Wrap { get; set; }
	}

}
