using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class CheckBox : BaseStyledComponent
	{
		private string _inputId => ClientID;

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
		public EnumParameter<TextAlign> TextAlign { get; set; } = Enums.TextAlign.Right;

		[Parameter]
		public EventCallback<ChangeEventArgs> OnCheckedChanged { get; set; }

		/// <summary>
		/// Gets or sets whether the control automatically posts back when the value changes.
		/// In SSR mode, emits <c>onchange="this.form.submit()"</c> on the HTML element.
		/// In Interactive mode, Blazor's native event binding handles change events automatically.
		/// </summary>
		[Parameter]
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
