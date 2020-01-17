using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class FooterStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "FooterStyle")]
		protected TableItemStyle theFooterStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}

	}
}
