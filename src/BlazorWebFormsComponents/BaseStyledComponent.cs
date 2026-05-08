using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public abstract class BaseStyledComponent : BaseWebFormsComponent, IStyle
	{
		private static readonly Regex UnitFactoryRegex = new(@"^Unit\.(?<factory>Pixel|Point|Percentage)\((?<value>-?[0-9]+(?:\.[0-9]+)?)\)$", RegexOptions.Compiled);
		private Unit _height;
		private Unit _width;

		[Parameter]
		public WebColor BackColor { get; set; }

		[Parameter]
		public WebColor BorderColor { get; set; }

		[Parameter]
		public EnumParameter<BorderStyle> BorderStyle { get; set; }

		[Parameter]
		public Unit BorderWidth { get; set; }

		[Parameter]
		public string CssClass { get; set; }

		[Parameter]
		public WebColor ForeColor { get; set; }

		[Parameter]
		public string Height
		{
			get => _height.ToString();
			set => _height = ParseUnitString(value, nameof(Height));
		}

		[Parameter]
		public string Width
		{
			get => _width.ToString();
			set => _width = ParseUnitString(value, nameof(Width));
		}

		Unit IHasLayoutStyle.Height
		{
			get => _height;
			set => _height = value;
		}

		Unit IHasLayoutStyle.Width
		{
			get => _width;
			set => _width = value;
		}

		[Parameter]
		public FontInfo Font { get; set; } = new FontInfo();

		protected string Style => this.ToStyle().Build().NullIfEmpty();

		private static Unit ParseUnitString(string value, string parameterName)
		{
			if (string.IsNullOrWhiteSpace(value))
				return Unit.Empty;

			var trimmed = value.Trim();
			var factoryMatch = UnitFactoryRegex.Match(trimmed);
			if (factoryMatch.Success)
			{
				var numericText = factoryMatch.Groups["value"].Value;
				var factory = factoryMatch.Groups["factory"].Value;
				return factory switch
				{
					nameof(Unit.Pixel) => Unit.Pixel(int.Parse(numericText, CultureInfo.InvariantCulture)),
					nameof(Unit.Point) => Unit.Point(int.Parse(numericText, CultureInfo.InvariantCulture)),
					nameof(Unit.Percentage) => Unit.Percentage(double.Parse(numericText, CultureInfo.InvariantCulture)),
					_ => throw new InvalidOperationException($"Unsupported unit factory syntax for {parameterName}: {value}")
				};
			}

			try
			{
				return Unit.Parse(trimmed, CultureInfo.InvariantCulture);
			}
			catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
			{
				throw new InvalidOperationException($"Could not parse {parameterName} value '{value}'.", ex);
			}
		}

		/// <summary>
		/// Applies skin properties respecting the specified ThemeMode.
		/// StyleSheetTheme mode: theme sets defaults, explicit values take precedence.
		/// Theme mode: theme overrides all values.
		/// </summary>
		protected override void ApplyThemeSkin(ControlSkin skin, ThemeMode mode)
		{
			if (mode == ThemeMode.Theme)
			{
				// Theme mode: override all properties where skin has a value
				if (skin.BackColor != default)
					BackColor = skin.BackColor;

				if (skin.ForeColor != default)
					ForeColor = skin.ForeColor;

				if (skin.BorderColor != default)
					BorderColor = skin.BorderColor;

				if (skin.BorderStyle.HasValue)
					BorderStyle = skin.BorderStyle.Value;

				if (skin.BorderWidth.HasValue)
					BorderWidth = skin.BorderWidth.Value;

				if (!string.IsNullOrEmpty(skin.CssClass))
					CssClass = skin.CssClass;

				if (skin.Height.HasValue)
					_height = skin.Height.Value;

				if (skin.Width.HasValue)
					_width = skin.Width.Value;

				if (!string.IsNullOrEmpty(skin.ToolTip))
					ToolTip = skin.ToolTip;

				if (skin.Font != null)
				{
					if (Font == null)
						Font = new FontInfo();

					if (!string.IsNullOrEmpty(skin.Font.Name))
						Font.Name = skin.Font.Name;

					if (skin.Font.Size != FontUnit.Empty)
						Font.Size = skin.Font.Size;

					if (skin.Font.Bold)
						Font.Bold = true;

					if (skin.Font.Italic)
						Font.Italic = true;

					if (skin.Font.Underline)
						Font.Underline = true;
				}
			}
			else
			{
				// StyleSheetTheme mode: only set if not already set
				if (BackColor == default && skin.BackColor != default)
					BackColor = skin.BackColor;

				if (ForeColor == default && skin.ForeColor != default)
					ForeColor = skin.ForeColor;

				if (BorderColor == default && skin.BorderColor != default)
					BorderColor = skin.BorderColor;

				if (BorderStyle.Value == default && skin.BorderStyle.HasValue)
					BorderStyle = skin.BorderStyle.Value;

				if (BorderWidth == default && skin.BorderWidth.HasValue)
					BorderWidth = skin.BorderWidth.Value;

				if (string.IsNullOrEmpty(CssClass) && !string.IsNullOrEmpty(skin.CssClass))
					CssClass = skin.CssClass;

				if (_height == default && skin.Height.HasValue)
					_height = skin.Height.Value;

				if (_width == default && skin.Width.HasValue)
					_width = skin.Width.Value;

				if (string.IsNullOrEmpty(ToolTip) && !string.IsNullOrEmpty(skin.ToolTip))
					ToolTip = skin.ToolTip;

				if (skin.Font != null)
				{
					if (Font == null)
						Font = new FontInfo();

					if (string.IsNullOrEmpty(Font.Name) && string.IsNullOrEmpty(Font.Names) && !string.IsNullOrEmpty(skin.Font.Name))
						Font.Name = skin.Font.Name;

					if (Font.Size == FontUnit.Empty && skin.Font.Size != FontUnit.Empty)
						Font.Size = skin.Font.Size;

					if (!Font.Bold && skin.Font.Bold)
						Font.Bold = true;

					if (!Font.Italic && skin.Font.Italic)
						Font.Italic = true;

					if (!Font.Underline && skin.Font.Underline)
						Font.Underline = true;
				}
			}
		}
	}
}
