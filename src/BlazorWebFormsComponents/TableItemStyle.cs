using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public class TableItemStyle : Style, ITableItemStyle
	{

		internal TableItemStyle() { }

		public HorizontalAlign HorizontalAlign { get; set; }

		public VerticalAlign VerticalAlign { get; set; }

		public bool Wrap { get; set; } = true;

		public override string ToString()
		{

			return ((ITableItemStyle)this).ToStyle().NullIfEmpty();

		}

	}

}
