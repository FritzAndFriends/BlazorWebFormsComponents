using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BlazorWebFormsComponents
{
	public class TableItemStyle
	{

		internal TableItemStyle() { }

		public Color BackColor { get; set; }

		public Color BorderColor { get; set; }

		public BorderStyle BorderStyle { get; set; }

		public Unit BorderWidth { get; set; }

		public override string ToString()
		{

			var sb = new StringBuilder();
			if (BackColor != default(Color)) sb.Append($"background-color: {(BackColor.IsNamedColor ? BackColor.Name : ColorTranslator.ToHtml(BackColor))}");

			return sb.ToString();

		}

	}
}
