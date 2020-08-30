using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class HyperLink : BaseWebFormsComponent, IHasStyle
	{
		[Parameter]
		public string NavigationUrl { get; set; }

		[Parameter]
		public string Target { get; set; } = string.Empty;

		[Parameter] 
		public string Text { get; set; }

		[Parameter] public string ToolTip { get; set; }

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public FontInfo Font { get; set; } = new FontInfo();

		private string CalculatedStyle => this.ToStyle().Build().NullIfEmpty();

	}
}
