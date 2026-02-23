using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class CheckBox : BaseStyledComponent
	{
		private string _inputId => !string.IsNullOrEmpty(ClientID) ? ClientID : _generatedInputId;
		private readonly string _generatedInputId = Guid.NewGuid().ToString("N");

		[Parameter]
		public bool CausesValidation { get; set; } = true;

		[Parameter]
		public string ValidationGroup { get; set; }

		[CascadingParameter(Name = "ValidationGroupCoordinator")]
		protected ValidationGroupCoordinator Coordinator { get; set; }

		[Parameter]
		public bool Checked { get; set; }

		[Parameter]
		public EventCallback<bool> CheckedChanged { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public TextAlign TextAlign { get; set; } = TextAlign.Right;

		[Parameter]
		public EventCallback<ChangeEventArgs> OnCheckedChanged { get; set; }

		[Parameter, Obsolete("AutoPostBack is not supported in Blazor. Use OnCheckedChanged event instead.")]
		public bool AutoPostBack { get; set; }

		private async Task HandleChange(ChangeEventArgs e)
		{
			Checked = (bool)e.Value;

			if (CausesValidation && Coordinator != null)
			{
				Coordinator.ValidateGroup(ValidationGroup);
			}

			await CheckedChanged.InvokeAsync(Checked);
			await OnCheckedChanged.InvokeAsync(e);
		}
	}
}
