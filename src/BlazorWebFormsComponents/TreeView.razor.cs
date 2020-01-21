using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class TreeView : BaseWebFormsComponent, IHasStyle
	{

		[Parameter]
		public RenderFragment Nodes { get; set; }

		[Parameter]
		public TreeNodeTypes ShowCheckBoxes { get; set; }

		[Parameter]
		public bool ShowExpandCollapse { get; set; } = true;

		#region IHasStyle

		[Parameter]public WebColor BackColor { get; set; }
		[Parameter]public WebColor BorderColor { get; set; }
		[Parameter]public BorderStyle BorderStyle { get; set; }
		[Parameter]public Unit BorderWidth { get; set; }
		[Parameter]public string CssClass { get; set; }
		[Parameter]public WebColor ForeColor { get; set; }
		[Parameter]public Unit Height { get; set; }
		[Parameter]public Unit Width { get; set; }
		[Parameter]public bool Font_Bold { get; set; }
		[Parameter]public bool Font_Italic { get; set; }
		[Parameter]public string Font_Names { get; set; }
		[Parameter]public bool Font_Overline { get; set; }
		[Parameter]public FontUnit Font_Size { get; set; }
		[Parameter]public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		#endregion

	}
}
