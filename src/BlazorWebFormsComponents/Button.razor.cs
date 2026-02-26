using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
	public partial class Button : ButtonBaseComponent
	{
		internal string CalculatedButtonType => UseSubmitBehavior ? "submit" : "button";

		internal string CalculatedCssClass => Enabled ? CssClass : string.Concat(CssClass, " aspNetDisabled").Trim();

		[Obsolete("In Blazor PostbackURL is not supported")]
		public override string PostBackUrl { get; set; }

		[Parameter]
		public bool UseSubmitBehavior { get; set; } = true;

	}
}
