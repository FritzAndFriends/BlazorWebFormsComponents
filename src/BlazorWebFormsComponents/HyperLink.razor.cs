using System;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class HyperLink : BaseStyledComponent
	{
		[Parameter]
		public string NavigateUrl { get; set; }

		[Obsolete("Use NavigateUrl instead")]
		public string NavigationUrl
		{
			get => NavigateUrl;
			set => NavigateUrl = value;
		}

		[Parameter]
		public string Target { get; set; } = string.Empty;

		[Parameter] 
		public string Text { get; set; }

		[Parameter] public string ToolTip { get; set; }
	}
}
