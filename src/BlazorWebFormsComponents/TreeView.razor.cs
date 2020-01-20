using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class TreeView 
	{

		[Parameter]
		public RenderFragment Nodes { get; set; }

		[Parameter]
		public bool ShowExpandCollapse { get; set; } = true;


	}
}
