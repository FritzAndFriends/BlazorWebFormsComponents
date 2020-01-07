using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Drawing;

namespace BlazorWebFormsComponents
{
	public abstract class UiStyle : ComponentBase, IHasLayoutStyle
	{

		protected TableItemStyle theStyle { get; set; }


		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string, object> AdditionalAttributes { get; set; }

		[Parameter]
		public Color BackColor { get; set; }

		[Parameter]
		public Color BorderColor { get; set; }

		[Parameter]
		public BorderStyle BorderStyle { get; set; }

		[Parameter]
		public Unit BorderWidth { get; set; }

		[Parameter]
		public string CssClass { get; set; }

		[Parameter]
		public Color ForeColor { get; set; }

		[Parameter]
		public Unit Height { get; set; }

		[Parameter]
		public HorizontalAlign HorizontalAlign { get; set; }

		[Parameter]
		public VerticalAlign VerticalAlign { get; set; }

		[Parameter]
		public Unit Width { get; set; }


		//protected override void OnParametersSet()
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
				theStyle.HorizontalAlign = HorizontalAlign;
				theStyle.VerticalAlign = VerticalAlign;
				theStyle.Width = Width;

				theStyle.SetFontsFromAttributes(AdditionalAttributes);

			}


		}


	}
}
