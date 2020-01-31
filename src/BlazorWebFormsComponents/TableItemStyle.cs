using BlazorWebFormsComponents.Enums;
using BlazorComponentUtilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
