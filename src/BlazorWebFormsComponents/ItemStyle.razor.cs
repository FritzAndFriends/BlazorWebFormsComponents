using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class ItemStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ItemStyle")]
		protected TableItemStyle theItemStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}


	}
}
