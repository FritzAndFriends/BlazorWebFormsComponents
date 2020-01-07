﻿using Microsoft.AspNetCore.Components;
using System;
using System.Text;

namespace BlazorWebFormsComponents
{

	public partial class HeaderStyle : UiStyle
	{

		[CascadingParameter(Name = "HeaderStyle")]
		protected TableItemStyle theHeaderStyle { 
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}

	}
}
