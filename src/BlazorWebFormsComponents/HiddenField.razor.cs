using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
	public partial class HiddenField : BaseWebFormsComponent
	{
		[Parameter]
		public string Value { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnValueChanged { get; set; }

		protected void Changed()
		{
			OnValueChanged.InvokeAsync(new EventArgs());
		}
	}
}
