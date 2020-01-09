using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BlazorWebFormsComponents.Enums;
using System.Drawing;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
	public partial class AspNetValidationSummary : BaseWebFormsComponent, IHasStyle
	{
		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ValidationSummaryDisplayMode DisplayMode { get; set; } = ValidationSummaryDisplayMode.BulletList;
		[Parameter] public Color BackColor { get; set; }
		[Parameter] public Color BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public Color ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public HorizontalAlign HorizontalAlign { get; set; }
		[Parameter] public VerticalAlign VerticalAlign { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		protected string CalculatedStyle { get; set; }

		public bool IsValid => CurrentEditContext.GetValidationMessages()?.Any() ?? false;
		protected override void OnParametersSet()
		{

			CurrentEditContext.OnValidationStateChanged += (sender, eventArgs) => StateHasChanged();

		}

		protected override void OnInitialized()
		{

			this.SetFontsFromAttributes(AdditionalAttributes);

			var styleBuilder = new StringBuilder();

			this.ToStyleString(styleBuilder);

			CalculatedStyle = styleBuilder.ToString();

			base.OnInitialized();
		}
	}
}
