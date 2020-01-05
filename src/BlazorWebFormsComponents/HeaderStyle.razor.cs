using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BlazorWebFormsComponents
{

	public abstract class UiStyle : ComponentBase
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

				if (AdditionalAttributes != null)
				{
					if (AdditionalAttributes.ContainsKey("Font-Bold")) theStyle.Font_Bold = bool.Parse(AdditionalAttributes["Font-Bold"].ToString());
					if (AdditionalAttributes.ContainsKey("Font-Italic")) theStyle.Font_Italic = bool.Parse(AdditionalAttributes["Font-Italic"].ToString());
					if (AdditionalAttributes.ContainsKey("Font-Names")) theStyle.Font_Names = AdditionalAttributes["Font-Names"].ToString();
					if (AdditionalAttributes.ContainsKey("Font-Overline")) theStyle.Font_Overline = bool.Parse(AdditionalAttributes["Font-Overline"].ToString());
					if (AdditionalAttributes.ContainsKey("Font-Size")) theStyle.Font_Size = FontUnit.Parse(AdditionalAttributes["Font-Size"].ToString());
					if (AdditionalAttributes.ContainsKey("Font-Strikeout")) theStyle.Font_Strikeout = bool.Parse(AdditionalAttributes["Font-Strikeout"].ToString());
					if (AdditionalAttributes.ContainsKey("Font-Underline")) theStyle.Font_Underline = bool.Parse(AdditionalAttributes["Font-Underline"].ToString());
				}

			}


		}


	}

	public partial class HeaderStyle : UiStyle
	{

		[CascadingParameter(Name = "HeaderStyle")]
		protected TableItemStyle theHeaderStyle { 
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}

	}
}
