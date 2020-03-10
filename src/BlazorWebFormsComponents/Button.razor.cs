using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
  public partial class Button : BaseWebFormsComponent
  {
		[Parameter] public string Text { get; set; }

		[Parameter]
		public Action OnClick{ get; set; }

		private async Task TriggerClick()
		{
			OnClick?.Invoke();
		}


	}
}
