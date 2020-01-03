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

		public Color ForeColor { get; set; }

		public override string ToString()
		{

			var sb = new StringBuilder();
			if (BackColor != default(Color)) sb.Append($"background-color: {ColorTranslator.ToHtml(BackColor)};");
			if (ForeColor != default(Color)) sb.Append($"color: {ColorTranslator.ToHtml(ForeColor)};");
			if (BorderStyle != BorderStyle.None && BorderStyle != BorderStyle.NotSet && BorderWidth.Value > 0 && BorderColor != default(Color)) {

				sb.Append($"border: {BorderWidth.ToString()} {BorderStyle.ToString().ToLowerInvariant()} {ColorTranslator.ToHtml(BorderColor)}");

			}


			return sb.ToString();

		}

		public void FromAttributes(IEnumerable<KeyValuePair<string,object>> attributes) 
		{

			var headerStyleType = typeof(TableItemStyle);
			foreach (var itemStyle in attributes)
			{
				var propInfo = headerStyleType.GetProperty(itemStyle.Key);

				// TODO:  Make this a more comprehensive type-cast appropriately

				var outValue = propInfo.PropertyType == typeof(Color) ? itemStyle.Value.GetColorFromHtml() : itemStyle.Value;
				propInfo.SetValue(this, Convert.ChangeType(outValue, propInfo.PropertyType));
			}
		}

	}
}
