using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class RadioButton : BaseStyledComponent
	{
		private string _inputId => !string.IsNullOrEmpty(ClientID) ? ClientID : _generatedInputId;
		private readonly string _generatedInputId = Guid.NewGuid().ToString("N");

		[Parameter]
		public bool Checked { get; set; }

		[Parameter]
		public EventCallback<bool> CheckedChanged { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string GroupName { get; set; }

		[Parameter]
		public Enums.TextAlign TextAlign { get; set; } = Enums.TextAlign.Right;

		[Parameter]
		public EventCallback<ChangeEventArgs> OnCheckedChanged { get; set; }

		[Parameter, Obsolete("AutoPostBack is not supported in Blazor. Use OnCheckedChanged event instead.")]
		public bool AutoPostBack { get; set; }

		private string EffectiveGroupName => string.IsNullOrEmpty(GroupName) ? _inputId : GroupName;

		private async Task HandleChange(ChangeEventArgs e)
		{
			Checked = true;
			await CheckedChanged.InvokeAsync(Checked);
			await OnCheckedChanged.InvokeAsync(e);
		}
	}
}
