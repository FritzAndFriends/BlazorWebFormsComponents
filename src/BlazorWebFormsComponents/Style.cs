using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public class Style : IHasStyle
	{

		internal Style() { }

		public WebColor BackColor { get; set; }

		public WebColor BorderColor { get; set; }

		public BorderStyle BorderStyle { get; set; }

		public Unit BorderWidth { get; set; }

		public string CssClass { get; set; }

		public bool Font_Bold { get; set; }

		public bool Font_Italic { get; set; }

		public string Font_Names { get; set; }

		public bool Font_Overline { get; set; }

		public FontUnit Font_Size { get; set; } = FontUnit.Empty;

		public bool Font_Strikeout { get; set; }

		public bool Font_Underline { get; set; }

		public WebColor ForeColor { get; set; }

		public Unit Height { get; set; }

		public Unit Width { get; set; }

		public override string ToString() => this.ToStyle().NullIfEmpty();

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

				var outValue = propInfo.PropertyType switch
				{
					null => null,
					{ Name: nameof(WebColor) } => itemStyle.Value.GetWebColorFromHtml(),
					{ Name: nameof(Unit) } => new Unit(itemStyle.Value.ToString()),
					{ Name: nameof(FontUnit) } => FontUnit.Parse(itemStyle.Value.ToString()),
					{ IsEnum: true } => Enum.Parse(propInfo.PropertyType, itemStyle.Value.ToString()),
					_ => itemStyle.Value
				};

				propInfo.SetValue(this, Convert.ChangeType(outValue, propInfo.PropertyType));

			}
		}

	}

}
