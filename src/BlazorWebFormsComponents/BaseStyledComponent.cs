using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public abstract class BaseStyledComponent : BaseWebFormsComponent, IStyle
	{
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
		public Unit Height { get; set; }

		[Parameter]
		public Unit Width { get; set; }

		[Parameter]
		public FontInfo Font { get; set; } = new FontInfo();

		protected string Style => this.ToStyle().Build().NullIfEmpty();

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
					Height = skin.Height.Value;

				if (skin.Width.HasValue)
					Width = skin.Width.Value;

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

				if (Height == default && skin.Height.HasValue)
					Height = skin.Height.Value;

				if (Width == default && skin.Width.HasValue)
					Width = skin.Width.Value;

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
