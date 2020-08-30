using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
namespace BlazorWebFormsComponents
{
	public partial class ImageButton : ButtonBaseComponent, IImageComponent, IHasStyle
	{
		[Parameter]
		public string AlternateText { get; set; }

		[Parameter]
		public string DescriptionUrl { get; set; } = string.Empty;

		[Parameter]
		public ImageAlign ImageAlign { get; set; }

		[Parameter]
		public string ImageUrl { get; set; }

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

		[Parameter]
		public string ToolTip { get; set; }

		private string CalculatedStyle => this.ToStyle().Build().NullIfEmpty();
	}
}
