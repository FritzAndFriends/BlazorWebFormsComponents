using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class AlternatingItemStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "AlternatingItemStyle")]
		protected TableItemStyle theAlternatingItemStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}


	}
}
