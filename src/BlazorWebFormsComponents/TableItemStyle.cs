using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

		public string CssClass { get; set; }

		public bool Font_Bold { get; set; }

		public bool Font_Italic { get; set; }

		public string Font_Name { get; set; }

		public string Font_Names { get; set; }

		public bool Font_Overline { get; set; }

		public FontUnit Font_Size { get; set; }

		public bool Font_Strikeout { get; set; }

		public bool Font_Underline { get; set; }

		public Color ForeColor { get; set; }

		public override string ToString()
		{

			var sb = new StringBuilder();
			if (BackColor != default(Color)) sb.Append($"background-color: {ColorTranslator.ToHtml(BackColor)};");
			if (ForeColor != default(Color)) sb.Append($"color: {ColorTranslator.ToHtml(ForeColor)};");
			if (BorderStyle != BorderStyle.None && BorderStyle != BorderStyle.NotSet && BorderWidth.Value > 0 && BorderColor != default(Color)) {

				sb.Append($"border: {BorderWidth.ToString()} {BorderStyle.ToString().ToLowerInvariant()} {ColorTranslator.ToHtml(BorderColor)};");

			}

			if (Font_Bold) sb.Append("font-weight:bold;");
			if (Font_Italic) sb.Append("font-style:italic;");

			return sb.Length == 0 ? null : sb.ToString();

		}

		public void FromUnknownAttributes(Dictionary<string,object> attributes, string prefix) {

			if (attributes?.Count > 0)
			{

				var theStyles = attributes.Keys
					.Where(k => k.StartsWith(prefix))
					.Select(k => new KeyValuePair<string, object>(k.Substring(prefix.Length), attributes[k]));

				this.FromAttributes(theStyles);

			}

		}

		private void FromAttributes(IEnumerable<KeyValuePair<string,object>> attributes) 
		{

			var headerStyleType = typeof(TableItemStyle);
			foreach (var itemStyle in attributes)
			{

				var formattedKey = itemStyle.Key.Replace("-", "_");
				var propInfo = headerStyleType.GetProperty(formattedKey);

				object outValue = propInfo.PropertyType switch
				{
					null => null,
					{ Name: nameof(Color) } => itemStyle.Value.GetColorFromHtml(),
					{ Name: nameof(Unit) } => new Unit(itemStyle.Value.ToString()),
					{ Name: nameof(FontUnit) } => FontUnit.Parse(itemStyle.Value),
					{ IsEnum: true } => Enum.Parse(propInfo.PropertyType, itemStyle.Value.ToString()),
					_ => itemStyle.Value
				};

				propInfo.SetValue(this, Convert.ChangeType(outValue, propInfo.PropertyType));

			}
		}

	}
}
