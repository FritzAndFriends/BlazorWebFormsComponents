using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
	public partial class Button : ButtonBaseComponent, IHasStyle
	{

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		private string CalculatedStyle => this.ToStyle().Build().NullIfEmpty();

		internal string CalculatedButtonType => CausesValidation ? "submit" : "button";

		internal string CalculatedCssClass => Enabled ? CssClass : string.Concat(CssClass, " aspNetDisabled").Trim();

		[Obsolete("In Blazor PostbackURL is not supported")]
		public override string PostBackUrl { get; set; }

		[Parameter, Obsolete("In Blazor this behaves the same whether activated or not")]
		public bool UseSubmitBehavior { get; set; }

		[Parameter]
		public string ToolTip { get; set; }

	}
}
