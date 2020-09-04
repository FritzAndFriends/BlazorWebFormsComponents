using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public class TableItemStyle : Style, IHasTableItemStyle
	{

		internal TableItemStyle() { }

		public HorizontalAlign HorizontalAlign { get; set; }

		public VerticalAlign VerticalAlign { get; set; }

		public bool Wrap { get; set; } = true;

		public override string ToString()
		{

			return ((IHasTableItemStyle)this).ToStyle().NullIfEmpty();

		}

	}

}
