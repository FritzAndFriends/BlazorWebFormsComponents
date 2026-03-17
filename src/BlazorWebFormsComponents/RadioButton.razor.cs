using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class RadioButton : BaseStyledComponent
	{
		private string _inputId => ClientID;
		private readonly string _generatedGroupName = Guid.NewGuid().ToString("N");

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
		public string GroupName { get; set; }

		[Parameter]
		public EnumParameter<Enums.TextAlign> TextAlign { get; set; } = Enums.TextAlign.Right;

		[Parameter]
		public EventCallback<ChangeEventArgs> OnCheckedChanged { get; set; }

		[Parameter, Obsolete("AutoPostBack is not supported in Blazor. Use OnCheckedChanged event instead.")]
		public bool AutoPostBack { get; set; }

		private string EffectiveGroupName => !string.IsNullOrEmpty(GroupName)
			? GroupName
			: (!string.IsNullOrEmpty(ClientID) ? ClientID : _generatedGroupName);

		private async Task HandleChange(ChangeEventArgs e)
		{
			Checked = true;

			if (CausesValidation && Coordinator != null)
			{
				Coordinator.ValidateGroup(ValidationGroup);
			}

			await CheckedChanged.InvokeAsync(Checked);
			await OnCheckedChanged.InvokeAsync(e);
		}
	}
}
