using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
	public partial class Button : ButtonBaseComponent
	{
		internal string CalculatedButtonType => CausesValidation ? "submit" : "button";

		[Obsolete("In Blazor PostbackURL is not supported")]
		public override string PostBackUrl { get; set; }

		[Parameter, Obsolete("In Blazor this behaves the same whether activated or not")]
		public bool UseSubmitBehavior { get; set; }

		[Parameter]
		public string ToolTip { get; set; }
	}
}
