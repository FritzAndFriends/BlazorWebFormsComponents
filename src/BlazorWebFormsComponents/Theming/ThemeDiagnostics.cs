using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Provides validation and diagnostic helpers for theme configurations.
	/// </summary>
	public static class ThemeDiagnostics
	{
		/// <summary>
		/// Gets a list of all known control type names in BlazorWebFormsComponents.
		/// Used for validation — warns if a theme references an unknown control type.
		/// </summary>
		public static IReadOnlySet<string> KnownControlTypes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"AdRotator",
			"BulletedList",
			"Button",
			"Calendar",
			"Chart",
			"CheckBox",
			"CheckBoxList",
			"Content",
			"ContentPlaceHolder",
			"DataGrid",
			"DataList",
			"DataPager",
			"DetailsView",
			"DropDownList",
			"FileUpload",
			"FormView",
			"GridView",
			"HiddenField",
			"HyperLink",
			"Image",
			"ImageButton",
			"ImageMap",
			"Label",
			"LinkButton",
			"ListBox",
			"ListView",
			"Literal",
			"Localize",
			"MasterPage",
			"Menu",
			"MultiView",
			"Page",
			"Panel",
			"PlaceHolder",
			"RadioButton",
			"RadioButtonList",
			"Repeater",
			"ScriptManager",
			"ScriptManagerProxy",
			"SiteMapPath",
			"Substitution",
			"Table",
			"TextBox",
			"Timer",
			"TreeView",
			"UpdatePanel",
			"UpdateProgress",
			"View",
			"WebFormsPage"
		};

		/// <summary>
		/// Gets known sub-style names per control type.
		/// </summary>
		public static IReadOnlyDictionary<string, IReadOnlySet<string>> KnownSubStyles { get; } = new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
		{
			["GridView"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"HeaderStyle",
				"RowStyle",
				"AlternatingRowStyle",
				"FooterStyle",
				"PagerStyle",
				"EditRowStyle",
				"SelectedRowStyle",
				"EmptyDataRowStyle"
			},
			["DetailsView"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"HeaderStyle",
				"RowStyle",
				"AlternatingRowStyle",
				"FooterStyle",
				"CommandRowStyle",
				"EditRowStyle",
				"InsertRowStyle",
				"FieldHeaderStyle",
				"EmptyDataRowStyle",
				"PagerStyle"
			},
			["FormView"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"HeaderStyle",
				"RowStyle",
				"EditRowStyle",
				"InsertRowStyle",
				"FooterStyle",
				"PagerStyle",
				"EmptyDataRowStyle"
			},
			["DataGrid"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"HeaderStyle",
				"ItemStyle",
				"AlternatingItemStyle",
				"FooterStyle",
				"PagerStyle",
				"SelectedItemStyle",
				"EditItemStyle"
			},
			["DataList"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"HeaderStyle",
				"FooterStyle",
				"ItemStyle",
				"AlternatingItemStyle",
				"SeparatorStyle"
			},
			["Menu"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"StaticMenuItemStyle",
				"StaticSelectedStyle",
				"StaticHoverStyle",
				"DynamicMenuItemStyle",
				"DynamicSelectedStyle",
				"DynamicHoverStyle"
			},
			["TreeView"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"RootNodeStyle",
				"ParentNodeStyle",
				"LeafNodeStyle",
				"NodeStyle",
				"SelectedNodeStyle",
				"HoverNodeStyle"
			},
			["Calendar"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"TitleStyle",
				"NextPrevStyle",
				"DayHeaderStyle",
				"DayStyle",
				"TodayDayStyle",
				"SelectedDayStyle",
				"OtherMonthDayStyle",
				"WeekendDayStyle",
				"SelectorStyle"
			}
		};

		/// <summary>
		/// Validates a ThemeConfiguration and returns a list of warnings.
		/// </summary>
		public static List<string> Validate(ThemeConfiguration config)
		{
			if (config is null)
				throw new ArgumentNullException(nameof(config));

			var warnings = new List<string>();

			// Get all control types and skins via reflection
			var configType = typeof(ThemeConfiguration);
			var skinsField = configType.GetField("_skins", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			
			if (skinsField == null)
			{
				warnings.Add("Unable to access internal skin collection for validation.");
				return warnings;
			}

			var skins = skinsField.GetValue(config) as Dictionary<string, Dictionary<string, ControlSkin>>;
			if (skins == null || skins.Count == 0)
			{
				warnings.Add("Theme configuration contains no skins.");
				return warnings;
			}

			foreach (var controlTypeEntry in skins)
			{
				var controlTypeName = controlTypeEntry.Key;
				var skinMap = controlTypeEntry.Value;

				// Rule 1: Unknown control types
				if (!KnownControlTypes.Contains(controlTypeName))
				{
					warnings.Add($"Unknown control type '{controlTypeName}' in theme configuration. This may be a typo or unsupported control.");
				}

				foreach (var skinEntry in skinMap)
				{
					var skinId = skinEntry.Key;
					var skin = skinEntry.Value;

					// Rule 3: Empty skins
					if (IsEmptySkin(skin))
					{
						var skinDescription = string.IsNullOrEmpty(skinId) 
							? $"default skin for '{controlTypeName}'"
							: $"skin '{skinId}' for '{controlTypeName}'";
						warnings.Add($"Empty skin detected: {skinDescription}. No properties are set, which may indicate misconfiguration.");
					}

					// Rule 2: Unknown sub-style names
					if (skin.SubStyles != null && skin.SubStyles.Count > 0)
					{
						if (KnownSubStyles.TryGetValue(controlTypeName, out var knownSubStyleSet))
						{
							foreach (var subStyleName in skin.SubStyles.Keys)
							{
								if (!knownSubStyleSet.Contains(subStyleName))
								{
									var skinDescription = string.IsNullOrEmpty(skinId)
										? $"default skin for '{controlTypeName}'"
										: $"skin '{skinId}' for '{controlTypeName}'";
									warnings.Add($"Unknown sub-style '{subStyleName}' in {skinDescription}. Known sub-styles for {controlTypeName}: {string.Join(", ", knownSubStyleSet)}");
								}
							}
						}
						else
						{
							// Control type doesn't support sub-styles
							var skinDescription = string.IsNullOrEmpty(skinId)
								? $"default skin for '{controlTypeName}'"
								: $"skin '{skinId}' for '{controlTypeName}'";
							warnings.Add($"Control type '{controlTypeName}' does not support sub-styles, but {skinDescription} defines sub-styles: {string.Join(", ", skin.SubStyles.Keys)}");
						}
					}
				}
			}

			return warnings;
		}

		/// <summary>
		/// Checks if a ControlSkin has no properties set.
		/// </summary>
		private static bool IsEmptySkin(ControlSkin skin)
		{
			if (skin == null)
				return true;

			return skin.BackColor.IsEmpty
				&& skin.ForeColor.IsEmpty
				&& skin.BorderColor.IsEmpty
				&& skin.BorderStyle == null
				&& skin.BorderWidth == null
				&& string.IsNullOrEmpty(skin.CssClass)
				&& skin.Height == null
				&& skin.Width == null
				&& skin.Font == null
				&& string.IsNullOrEmpty(skin.ToolTip)
				&& (skin.SubStyles == null || skin.SubStyles.Count == 0);
		}
	}
}
