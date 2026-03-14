using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public class TableItemStyle : Style, IHasTableItemStyle
	{

		internal TableItemStyle() { }

		public EnumParameter<HorizontalAlign> HorizontalAlign { get; set; }

		public EnumParameter<VerticalAlign> VerticalAlign { get; set; }

		public bool Wrap { get; set; } = true;

		public override string ToString()
		{

			return ((IHasTableItemStyle)this).ToStyle().NullIfEmpty();

		}

	}

}
