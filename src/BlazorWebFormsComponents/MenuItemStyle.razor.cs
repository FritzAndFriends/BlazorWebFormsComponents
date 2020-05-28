using System;
using System.Collections.Generic;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class MenuItemStyle : ComponentBase, IHasStyle
	{

		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string,object> OtherAttributes { get;set;}

		[CascadingParameter(Name="ParentMenu")]
		public Menu ParentMenu {get;set;}

		[Parameter]
		public WebColor BackColor { get; set; }
		[Parameter]
		public WebColor BorderColor { get; set; }
		[Parameter]
		public BorderStyle BorderStyle { get; set; }
		[Parameter]
		public Unit BorderWidth { get; set; }
		[Parameter]
		public string CssClass { get; set; }
		[Parameter]
		public WebColor ForeColor { get; set; }
		[Parameter]
		public Unit Height { get; set; }
		[Parameter]
		public Unit Width { get; set; }
		[Parameter]
		public bool Font_Bold { get; set; }
		[Parameter]
		public bool Font_Italic { get; set; }
		[Parameter]
		public string Font_Names { get; set; }
		[Parameter]
		public bool Font_Overline { get; set; }
		[Parameter]
		public FontUnit Font_Size { get; set; }
		[Parameter]
		public bool Font_Strikeout { get; set; }
		[Parameter]
		public bool Font_Underline { get; set; }

		protected void SetPropertiesFromUnknownAttributes() {

			var thisType = this.GetType();
			if (OtherAttributes == null) return;

			foreach (var itemStyle in OtherAttributes)
			{

				var formattedKey = itemStyle.Key.Replace("-", "_");
				var propInfo = thisType.GetProperty(formattedKey);
				if (propInfo == null) continue;

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

		protected override void OnInitialized() {
			SetPropertiesFromUnknownAttributes();
			base.OnInitialized();
		}

	}

	public class StaticMenuItemStyle : MenuItemStyle {

		protected override void OnInitialized() {
			base.OnInitialized();
			ParentMenu.StaticMenuItemStyle = this;
		}

	}

	public class StaticHoverStyle : MenuItemStyle {

		protected override void OnInitialized() {
			base.OnInitialized();
			ParentMenu.StaticHoverStyle = this;
		}

	}

	public class DynamicMenuItemStyle : MenuItemStyle {

		protected override void OnInitialized() {
			base.OnInitialized();
			ParentMenu.DynamicMenuItemStyle = this;
		}

	}

	public class DynamicHoverStyle : MenuItemStyle {

		protected override void OnInitialized() {
				base.OnInitialized();
				ParentMenu.DynamicHoverStyle = this;
		}

	}

}
