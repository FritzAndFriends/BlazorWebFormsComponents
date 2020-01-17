using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class SeparatorStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "SeparatorStyle")]
		protected TableItemStyle theSeparatorStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}


	}
}
