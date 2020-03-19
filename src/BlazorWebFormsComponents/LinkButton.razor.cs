using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class LinkButton : BaseWebFormsComponent
	{
		[Parameter]
		public string PostBackUrl { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public Action OnClick { get; set; }

		private async Task TriggerClick()
		{
			OnClick?.Invoke();
		}
	}
}
