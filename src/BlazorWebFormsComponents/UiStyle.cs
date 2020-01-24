using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	public abstract class UiStyle : UiStyle<Style>
	{

	}

	public abstract class UiStyle<TStyle> : ComponentBase, IHasLayoutStyle where TStyle : Style
	{

		protected TStyle theStyle { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string, object> AdditionalAttributes { get; set; }

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

		protected override void OnInitialized()
		{

			if (theStyle != null)
			{

				theStyle.BackColor = BackColor;
				theStyle.BorderColor = BorderColor;
				theStyle.BorderStyle = BorderStyle;
				theStyle.BorderWidth = BorderWidth;
				theStyle.CssClass = CssClass;

				theStyle.ForeColor = ForeColor;

				theStyle.Height = Height;
				theStyle.Width = Width;

				theStyle.SetFontsFromAttributes(AdditionalAttributes);

			}


		}

	}

}
