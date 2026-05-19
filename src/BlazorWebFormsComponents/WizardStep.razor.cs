using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
	public partial class WizardStep
	{

		[CascadingParameter]
		private Wizard ParentWizard { get; set; }

		[Parameter]
		public string Title { get; set; }

		[Parameter]
		public WizardStepType StepType { get; set; } = WizardStepType.Auto;

		[Parameter]
		public bool AllowReturn { get; set; } = true;

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override void OnInitialized()
		{
			base.OnInitialized();
			ParentWizard?.AddStep(this);
		}

	}
}
