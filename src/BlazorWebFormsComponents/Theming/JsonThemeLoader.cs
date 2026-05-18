using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Loads ThemeConfiguration from JSON.
	/// </summary>
	public static class JsonThemeLoader
	{
		private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
			Converters =
			{
				new WebColorConverter(),
				new UnitConverter(),
				new FontUnitConverter(),
				new BorderStyleConverter(),
				new FontInfoConverter()
			}
		};

		/// <summary>
		/// Parses a JSON string into a ThemeConfiguration.
		/// </summary>
		public static ThemeConfiguration FromJson(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
				throw new ArgumentException("JSON cannot be null or empty.", nameof(json));

			var dto = JsonSerializer.Deserialize<ThemeDto>(json, _options);
			return DtoToThemeConfiguration(dto);
		}

		/// <summary>
		/// Loads a ThemeConfiguration from a JSON file.
		/// </summary>
		public static ThemeConfiguration FromJsonFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

			if (!File.Exists(filePath))
				throw new FileNotFoundException($"Theme file not found: {filePath}", filePath);

			var json = File.ReadAllText(filePath);
			return FromJson(json);
		}

		/// <summary>
		/// Serializes a ThemeConfiguration to JSON.
		/// </summary>
		public static string ToJson(ThemeConfiguration config)
		{
			if (config is null)
				throw new ArgumentNullException(nameof(config));

			var dto = ThemeConfigurationToDto(config);
			return JsonSerializer.Serialize(dto, _options);
		}

		private static ThemeConfiguration DtoToThemeConfiguration(ThemeDto dto)
		{
			var config = new ThemeConfiguration();

			if (dto.Mode.HasValue)
				config.Mode = dto.Mode.Value;

			if (dto.CssFiles != null)
			{
				foreach (var cssFile in dto.CssFiles)
					config.WithCssFile(cssFile);
			}

			if (dto.Controls != null)
			{
				foreach (var controlEntry in dto.Controls)
				{
					var controlTypeName = controlEntry.Key;
					foreach (var skinEntry in controlEntry.Value)
					{
						var skinId = skinEntry.Key == "default" ? null : skinEntry.Key;
						var skin = DtoToControlSkin(skinEntry.Value);
						config.AddSkin(controlTypeName, skin, skinId);
					}
				}
			}

			return config;
		}

		private static ControlSkin DtoToControlSkin(ControlSkinDto dto)
		{
			var skin = new ControlSkin
			{
				BackColor = dto.BackColor ?? WebColor.Empty,
				ForeColor = dto.ForeColor ?? WebColor.Empty,
				BorderColor = dto.BorderColor ?? WebColor.Empty,
				BorderStyle = dto.BorderStyle,
				BorderWidth = dto.BorderWidth,
				CssClass = dto.CssClass,
				Height = dto.Height,
				Width = dto.Width,
				Font = dto.Font,
				ToolTip = dto.ToolTip
			};

			if (dto.SubStyles != null)
			{
				skin.SubStyles = new Dictionary<string, TableItemStyle>(StringComparer.OrdinalIgnoreCase);
				foreach (var subStyleEntry in dto.SubStyles)
				{
					skin.SubStyles[subStyleEntry.Key] = DtoToTableItemStyle(subStyleEntry.Value);
				}
			}

			return skin;
		}

		private static TableItemStyle DtoToTableItemStyle(TableItemStyleDto dto)
		{
			var style = new TableItemStyle
			{
				BackColor = dto.BackColor ?? WebColor.Empty,
				ForeColor = dto.ForeColor ?? WebColor.Empty,
				BorderColor = dto.BorderColor ?? WebColor.Empty,
				CssClass = dto.CssClass,
				Font = dto.Font
			};

			if (dto.BorderStyle.HasValue)
				style.BorderStyle = dto.BorderStyle.Value;

			if (dto.BorderWidth.HasValue)
				style.BorderWidth = dto.BorderWidth.Value;

			if (dto.Height.HasValue)
				style.Height = dto.Height.Value;

			if (dto.Width.HasValue)
				style.Width = dto.Width.Value;

			if (dto.HorizontalAlign.HasValue)
				style.HorizontalAlign = dto.HorizontalAlign.Value;

			if (dto.VerticalAlign.HasValue)
				style.VerticalAlign = dto.VerticalAlign.Value;

			if (dto.Wrap.HasValue)
				style.Wrap = dto.Wrap.Value;

			return style;
		}

		private static ThemeDto ThemeConfigurationToDto(ThemeConfiguration config)
		{
			return new ThemeDto
			{
				Mode = config.Mode,
				CssFiles = config.CssFiles,
				Controls = new Dictionary<string, Dictionary<string, ControlSkinDto>>()
			};
		}

		#region DTOs

		private class ThemeDto
		{
			public ThemeMode? Mode { get; set; }
			public List<string> CssFiles { get; set; }
			public Dictionary<string, Dictionary<string, ControlSkinDto>> Controls { get; set; }
		}

		private class ControlSkinDto
		{
			public WebColor? BackColor { get; set; }
			public WebColor? ForeColor { get; set; }
			public WebColor? BorderColor { get; set; }
			public BorderStyle? BorderStyle { get; set; }
			public Unit? BorderWidth { get; set; }
			public string CssClass { get; set; }
			public Unit? Height { get; set; }
			public Unit? Width { get; set; }
			public FontInfo Font { get; set; }
			public string ToolTip { get; set; }
			public Dictionary<string, TableItemStyleDto> SubStyles { get; set; }
		}

		private class TableItemStyleDto
		{
			public WebColor? BackColor { get; set; }
			public WebColor? ForeColor { get; set; }
			public WebColor? BorderColor { get; set; }
			public BorderStyle? BorderStyle { get; set; }
			public Unit? BorderWidth { get; set; }
			public string CssClass { get; set; }
			public Unit? Height { get; set; }
			public Unit? Width { get; set; }
			public FontInfo Font { get; set; }
			public HorizontalAlign? HorizontalAlign { get; set; }
			public VerticalAlign? VerticalAlign { get; set; }
			public bool? Wrap { get; set; }
		}

		#endregion

		#region JSON Converters

		private class WebColorConverter : JsonConverter<WebColor>
		{
			public override WebColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Null)
					return WebColor.Empty;

				var value = reader.GetString();
				if (string.IsNullOrEmpty(value))
					return WebColor.Empty;

				return new WebColor(value);
			}

			public override void Write(Utf8JsonWriter writer, WebColor value, JsonSerializerOptions options)
			{
				if (value.IsEmpty)
					writer.WriteNullValue();
				else
					writer.WriteStringValue(value.ToHtml());
			}
		}

		private class UnitConverter : JsonConverter<Unit>
		{
			public override Unit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Null)
					return Unit.Empty;

				var value = reader.GetString();
				if (string.IsNullOrEmpty(value))
					return Unit.Empty;

				return Unit.Parse(value);
			}

			public override void Write(Utf8JsonWriter writer, Unit value, JsonSerializerOptions options)
			{
				if (value.IsEmpty)
					writer.WriteNullValue();
				else
					writer.WriteStringValue(value.ToString());
			}
		}

		private class FontUnitConverter : JsonConverter<FontUnit>
		{
			public override FontUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Null)
					return FontUnit.Empty;

				var value = reader.GetString();
				if (string.IsNullOrEmpty(value))
					return FontUnit.Empty;

				return FontUnit.Parse(value);
			}

			public override void Write(Utf8JsonWriter writer, FontUnit value, JsonSerializerOptions options)
			{
				if (value.IsEmpty)
					writer.WriteNullValue();
				else
					writer.WriteStringValue(value.ToString());
			}
		}

		private class BorderStyleConverter : JsonConverter<BorderStyle>
		{
			public override BorderStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Null)
					return BorderStyle.NotSet;

				var value = reader.GetString();
				if (string.IsNullOrEmpty(value))
					return BorderStyle.NotSet;

				if (Enum.TryParse<BorderStyle>(value, true, out var result))
					return result;

				throw new JsonException($"Invalid BorderStyle value: {value}");
			}

			public override void Write(Utf8JsonWriter writer, BorderStyle value, JsonSerializerOptions options)
			{
				if (value == BorderStyle.NotSet)
					writer.WriteNullValue();
				else
					writer.WriteStringValue(value.ToString());
			}
		}

		private class FontInfoConverter : JsonConverter<FontInfo>
		{
			public override FontInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Null)
					return null;

				if (reader.TokenType != JsonTokenType.StartObject)
					throw new JsonException("Expected StartObject token for FontInfo.");

				var fontInfo = new FontInfo();

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						return fontInfo;

					if (reader.TokenType != JsonTokenType.PropertyName)
						throw new JsonException("Expected PropertyName token.");

					var propertyName = reader.GetString();
					reader.Read();

					switch (propertyName?.ToLowerInvariant())
					{
						case "bold":
							fontInfo.Bold = reader.GetBoolean();
							break;
						case "italic":
							fontInfo.Italic = reader.GetBoolean();
							break;
						case "underline":
							fontInfo.Underline = reader.GetBoolean();
							break;
						case "overline":
							fontInfo.Overline = reader.GetBoolean();
							break;
						case "strikeout":
							fontInfo.Strikeout = reader.GetBoolean();
							break;
						case "name":
							fontInfo.Name = reader.GetString();
							break;
						case "names":
							fontInfo.Names = reader.GetString();
							break;
						case "size":
							var sizeValue = reader.GetString();
							if (!string.IsNullOrEmpty(sizeValue))
								fontInfo.Size = FontUnit.Parse(sizeValue);
							break;
						default:
							reader.Skip();
							break;
					}
				}

				throw new JsonException("Unexpected end of JSON.");
			}

			public override void Write(Utf8JsonWriter writer, FontInfo value, JsonSerializerOptions options)
			{
				if (value is null)
				{
					writer.WriteNullValue();
					return;
				}

				writer.WriteStartObject();

				if (value.Bold)
					writer.WriteBoolean("bold", value.Bold);

				if (value.Italic)
					writer.WriteBoolean("italic", value.Italic);

				if (value.Underline)
					writer.WriteBoolean("underline", value.Underline);

				if (value.Overline)
					writer.WriteBoolean("overline", value.Overline);

				if (value.Strikeout)
					writer.WriteBoolean("strikeout", value.Strikeout);

				if (!string.IsNullOrEmpty(value.Name))
					writer.WriteString("name", value.Name);

				if (!string.IsNullOrEmpty(value.Names) && value.Names != value.Name)
					writer.WriteString("names", value.Names);

				if (!value.Size.IsEmpty)
					writer.WriteString("size", value.Size.ToString());

				writer.WriteEndObject();
			}
		}

		#endregion
	}
}
