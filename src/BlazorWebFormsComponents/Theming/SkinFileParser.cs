using BlazorWebFormsComponents.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Parses ASP.NET Web Forms .skin files into ThemeConfiguration objects.
	/// </summary>
	public static class SkinFileParser
	{
		private static readonly Regex CommentRegex = new Regex(@"<%--.*?--%>", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// Parses a .skin file from a string and adds entries to the given ThemeConfiguration.
		/// </summary>
		/// <param name="skinContent">The content of the .skin file as a string.</param>
		/// <param name="config">Optional existing ThemeConfiguration to add to. If null, creates a new one.</param>
		/// <returns>The ThemeConfiguration with parsed skin entries added.</returns>
		public static ThemeConfiguration ParseSkinFile(string skinContent, ThemeConfiguration config = null)
		{
			config ??= new ThemeConfiguration();

			if (string.IsNullOrWhiteSpace(skinContent))
				return config;

			try
			{
				// Strip ASP.NET comments
				var cleaned = CommentRegex.Replace(skinContent, string.Empty);

				// Wrap in a root element to make it valid XML
				cleaned = $"<root>{cleaned}</root>";

				// Replace asp: prefix with asp_ to make it XML-safe
				cleaned = cleaned.Replace("<asp:", "<asp_").Replace("</asp:", "</asp_");

				// Parse as XML
				var doc = XDocument.Parse(cleaned);

				// Process each control element
				foreach (var element in doc.Root.Elements())
				{
					ProcessControlElement(element, config);
				}
			}
			catch (Exception ex)
			{
				// Log warning but don't throw - be defensive with real-world .skin files
				Console.WriteLine($"Warning: Failed to parse .skin file content: {ex.Message}");
			}

			return config;
		}

		/// <summary>
		/// Parses all .skin files in a directory (theme folder).
		/// </summary>
		/// <param name="folderPath">Path to the theme folder containing .skin files.</param>
		/// <param name="config">Optional existing ThemeConfiguration to add to. If null, creates a new one.</param>
		/// <returns>The ThemeConfiguration with parsed skin entries from all .skin files.</returns>
		public static ThemeConfiguration ParseThemeFolder(string folderPath, ThemeConfiguration config = null)
		{
			config ??= new ThemeConfiguration();

			if (!Directory.Exists(folderPath))
			{
				Console.WriteLine($"Warning: Theme folder not found: {folderPath}");
				return config;
			}

			var skinFiles = Directory.GetFiles(folderPath, "*.skin", SearchOption.AllDirectories);

			foreach (var skinFile in skinFiles)
			{
				ParseSkinFileFromPath(skinFile, config);
			}

			return config;
		}

		/// <summary>
		/// Parses a single .skin file from disk.
		/// </summary>
		/// <param name="filePath">Path to the .skin file.</param>
		/// <param name="config">Optional existing ThemeConfiguration to add to. If null, creates a new one.</param>
		/// <returns>The ThemeConfiguration with parsed skin entries added.</returns>
		public static ThemeConfiguration ParseSkinFileFromPath(string filePath, ThemeConfiguration config = null)
		{
			config ??= new ThemeConfiguration();

			if (!File.Exists(filePath))
			{
				Console.WriteLine($"Warning: Skin file not found: {filePath}");
				return config;
			}

			try
			{
				var content = File.ReadAllText(filePath);
				return ParseSkinFile(content, config);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to read .skin file '{filePath}': {ex.Message}");
			}

			return config;
		}

		private static void ProcessControlElement(XElement element, ThemeConfiguration config)
		{
			try
			{
				// Extract control type name (e.g., "asp_Button" -> "Button")
				var controlTypeName = element.Name.LocalName;
				if (controlTypeName.StartsWith("asp_", StringComparison.OrdinalIgnoreCase))
				{
					controlTypeName = controlTypeName.Substring(4);
				}

				// Extract SkinID if present
				string skinId = null;
				var skinIdAttr = element.Attributes()
					.FirstOrDefault(a => string.Equals(a.Name.LocalName, "SkinID", StringComparison.OrdinalIgnoreCase));
				if (skinIdAttr != null)
				{
					skinId = skinIdAttr.Value;
				}

				// Create the skin
				var skin = new ControlSkin();

				// Process attributes
				foreach (var attr in element.Attributes())
				{
					ProcessAttribute(attr, skin);
				}

				// Process nested style elements (e.g., HeaderStyle, RowStyle)
				foreach (var childElement in element.Elements())
				{
					ProcessSubStyle(childElement, skin);
				}

				// Add skin to configuration
				config.AddSkin(controlTypeName, skin, skinId);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to process control element '{element.Name}': {ex.Message}");
			}
		}

		private static void ProcessAttribute(XAttribute attr, ControlSkin skin)
		{
			var attrName = attr.Name.LocalName;
			var attrValue = attr.Value;

			// Ignore runat="server" and SkinID (already handled)
			if (string.Equals(attrName, "runat", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(attrName, "SkinID", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			try
			{
				// Handle Font-* attributes specially
				if (attrName.StartsWith("Font-", StringComparison.OrdinalIgnoreCase))
				{
					ProcessFontAttribute(attrName, attrValue, skin);
					return;
				}

				// Handle standard attributes
				switch (attrName.ToLowerInvariant())
				{
					case "backcolor":
						skin.BackColor = WebColor.FromHtml(attrValue);
						break;
					case "forecolor":
						skin.ForeColor = WebColor.FromHtml(attrValue);
						break;
					case "bordercolor":
						skin.BorderColor = WebColor.FromHtml(attrValue);
						break;
					case "borderstyle":
						if (Enum.TryParse<BorderStyle>(attrValue, ignoreCase: true, out var borderStyle))
							skin.BorderStyle = borderStyle;
						break;
					case "borderwidth":
						skin.BorderWidth = new Unit(attrValue);
						break;
					case "cssclass":
						skin.CssClass = attrValue;
						break;
					case "height":
						skin.Height = new Unit(attrValue);
						break;
					case "width":
						skin.Width = new Unit(attrValue);
						break;
					case "tooltip":
						skin.ToolTip = attrValue;
						break;
					default:
						// Silently ignore unknown attributes
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to parse attribute '{attrName}={attrValue}': {ex.Message}");
			}
		}

		private static void ProcessFontAttribute(string attrName, string attrValue, ControlSkin skin)
		{
			// Ensure Font object exists
			skin.Font ??= new FontInfo();

			var fontProp = attrName.Substring(5); // Remove "Font-" prefix

			try
			{
				switch (fontProp.ToLowerInvariant())
				{
					case "bold":
						if (bool.TryParse(attrValue, out var bold))
							skin.Font.Bold = bold;
						break;
					case "italic":
						if (bool.TryParse(attrValue, out var italic))
							skin.Font.Italic = italic;
						break;
					case "underline":
						if (bool.TryParse(attrValue, out var underline))
							skin.Font.Underline = underline;
						break;
					case "overline":
						if (bool.TryParse(attrValue, out var overline))
							skin.Font.Overline = overline;
						break;
					case "strikeout":
						if (bool.TryParse(attrValue, out var strikeout))
							skin.Font.Strikeout = strikeout;
						break;
					case "name":
						skin.Font.Name = attrValue;
						break;
					case "names":
						skin.Font.Names = attrValue;
						break;
					case "size":
						skin.Font.Size = FontUnit.Parse(attrValue);
						break;
					default:
						// Silently ignore unknown font attributes
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to parse font attribute '{attrName}={attrValue}': {ex.Message}");
			}
		}

		private static void ProcessSubStyle(XElement element, ControlSkin skin)
		{
			try
			{
				var styleName = element.Name.LocalName;

				// Create TableItemStyle for this sub-component
				var style = new TableItemStyle();

				// Process all attributes as style properties
				foreach (var attr in element.Attributes())
				{
					ProcessStyleAttribute(attr, style);
				}

				// Add to SubStyles dictionary
				skin.SubStyles ??= new Dictionary<string, TableItemStyle>(StringComparer.OrdinalIgnoreCase);
				skin.SubStyles[styleName] = style;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to process sub-style element '{element.Name}': {ex.Message}");
			}
		}

		private static void ProcessStyleAttribute(XAttribute attr, TableItemStyle style)
		{
			var attrName = attr.Name.LocalName;
			var attrValue = attr.Value;

			try
			{
				// Handle Font-* attributes specially
				if (attrName.StartsWith("Font-", StringComparison.OrdinalIgnoreCase))
				{
					var fontProp = attrName.Substring(5);
					style.Font ??= new FontInfo();

					switch (fontProp.ToLowerInvariant())
					{
						case "bold":
							if (bool.TryParse(attrValue, out var bold))
								style.Font.Bold = bold;
							break;
						case "italic":
							if (bool.TryParse(attrValue, out var italic))
								style.Font.Italic = italic;
							break;
						case "underline":
							if (bool.TryParse(attrValue, out var underline))
								style.Font.Underline = underline;
							break;
						case "overline":
							if (bool.TryParse(attrValue, out var overline))
								style.Font.Overline = overline;
							break;
						case "strikeout":
							if (bool.TryParse(attrValue, out var strikeout))
								style.Font.Strikeout = strikeout;
							break;
						case "name":
							style.Font.Name = attrValue;
							break;
						case "names":
							style.Font.Names = attrValue;
							break;
						case "size":
							style.Font.Size = FontUnit.Parse(attrValue);
							break;
					}
					return;
				}

				// Handle standard style attributes
				switch (attrName.ToLowerInvariant())
				{
					case "backcolor":
						style.BackColor = WebColor.FromHtml(attrValue);
						break;
					case "forecolor":
						style.ForeColor = WebColor.FromHtml(attrValue);
						break;
					case "bordercolor":
						style.BorderColor = WebColor.FromHtml(attrValue);
						break;
					case "borderstyle":
						if (Enum.TryParse<BorderStyle>(attrValue, ignoreCase: true, out var borderStyle))
							style.BorderStyle = borderStyle;
						break;
					case "borderwidth":
						style.BorderWidth = new Unit(attrValue);
						break;
					case "cssclass":
						style.CssClass = attrValue;
						break;
					case "height":
						style.Height = new Unit(attrValue);
						break;
					case "width":
						style.Width = new Unit(attrValue);
						break;
					case "horizontalalign":
						if (Enum.TryParse<HorizontalAlign>(attrValue, ignoreCase: true, out var hAlign))
							style.HorizontalAlign = hAlign;
						break;
					case "verticalalign":
						if (Enum.TryParse<VerticalAlign>(attrValue, ignoreCase: true, out var vAlign))
							style.VerticalAlign = vAlign;
						break;
					case "wrap":
						if (bool.TryParse(attrValue, out var wrap))
							style.Wrap = wrap;
						break;
					default:
						// Silently ignore unknown attributes
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to parse style attribute '{attrName}={attrValue}': {ex.Message}");
			}
		}
	}
}
