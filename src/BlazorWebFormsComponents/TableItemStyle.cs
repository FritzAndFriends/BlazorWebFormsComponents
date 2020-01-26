using BlazorWebFormsComponents.Enums;
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

			var theStyle = ((IHasTableItemStyle)this).ToStyleString();
			if (string.IsNullOrEmpty(theStyle)) return null;

			return theStyle;

		}

	}

}
