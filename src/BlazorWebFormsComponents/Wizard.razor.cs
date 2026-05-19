using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class Wizard : BaseStyledComponent
	{

		#region Steps

		private List<WizardStep> _steps = new();

		public IReadOnlyList<WizardStep> WizardStepsList => _steps;

		internal void AddStep(WizardStep step)
		{
			if (!_steps.Contains(step))
			{
				_steps.Add(step);
				StateHasChanged();
			}
		}

		#endregion

		#region Properties

		[Parameter]
		public int ActiveStepIndex { get; set; }

		[Parameter]
		public EventCallback<int> ActiveStepIndexChanged { get; set; }

		[Parameter]
		public bool DisplaySideBar { get; set; } = true;

		[Parameter]
		public bool DisplayCancelButton { get; set; }

		[Parameter]
		public string HeaderText { get; set; }

		[Parameter]
		public string StartNextButtonText { get; set; } = "Next";

		[Parameter]
		public string StepNextButtonText { get; set; } = "Next";

		[Parameter]
		public string StepPreviousButtonText { get; set; } = "Previous";

		[Parameter]
		public string FinishButtonText { get; set; } = "Finish";

		[Parameter]
		public string FinishPreviousButtonText { get; set; } = "Previous";

		[Parameter]
		public string FinishCompleteButtonText { get; set; } = "Finish";

		[Parameter]
		public string CancelButtonText { get; set; } = "Cancel";

		[Parameter]
		public string FinishDestinationPageUrl { get; set; }

		[Parameter]
		public string CancelDestinationPageUrl { get; set; }

		#endregion

		#region Templates

		[Parameter]
		public RenderFragment WizardSteps { get; set; }

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment SideBarTemplate { get; set; }

		[Parameter]
		public RenderFragment StartNavigationTemplate { get; set; }

		[Parameter]
		public RenderFragment StepNavigationTemplate { get; set; }

		[Parameter]
		public RenderFragment FinishNavigationTemplate { get; set; }

		#endregion

		#region Style Properties

		[Parameter]
		public TableItemStyle NavigationButtonStyle { get; set; }

		[Parameter]
		public TableItemStyle SideBarButtonStyle { get; set; }

		[Parameter]
		public TableItemStyle SideBarStyle { get; set; }

		[Parameter]
		public TableItemStyle HeaderStyle { get; set; }

		[Parameter]
		public TableItemStyle StepStyle { get; set; }

		[Parameter]
		public TableItemStyle NavigationStyle { get; set; }

		#endregion

		#region Events

		[Parameter]
		public EventCallback<EventArgs> OnActiveStepChanged { get; set; }

		[Parameter]
		public EventCallback<WizardNavigationEventArgs> OnNextButtonClick { get; set; }

		[Parameter]
		public EventCallback<WizardNavigationEventArgs> OnPreviousButtonClick { get; set; }

		[Parameter]
		public EventCallback<WizardNavigationEventArgs> OnFinishButtonClick { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnCancelButtonClick { get; set; }

		[Parameter]
		public EventCallback<WizardNavigationEventArgs> OnSideBarButtonClick { get; set; }

		#endregion

		#region Services

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		#endregion

		#region Calculated Properties

		private WizardStepType GetEffectiveStepType(int index)
		{
			if (index < 0 || index >= _steps.Count)
				return WizardStepType.Auto;

			var step = _steps[index];
			if (step.StepType != WizardStepType.Auto)
				return step.StepType;

			// Auto determination based on position
			if (index == 0)
				return WizardStepType.Start;

			// Check if this is the last non-Complete step
			var lastNonCompleteIndex = _steps.Count - 1;
			for (var i = _steps.Count - 1; i >= 0; i--)
			{
				if (_steps[i].StepType == WizardStepType.Complete)
					lastNonCompleteIndex = i - 1;
				else
					break;
			}

			if (index == lastNonCompleteIndex)
				return WizardStepType.Finish;

			return WizardStepType.Step;
		}

		private bool ShowPreviousButton
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Step || stepType == WizardStepType.Finish;
			}
		}

		private bool ShowNextButton
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Start || stepType == WizardStepType.Step;
			}
		}

		private bool ShowFinishButton
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Finish;
			}
		}

		private bool IsCompleteStep
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Complete;
			}
		}

		private string NextButtonText
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Start ? StartNextButtonText : StepNextButtonText;
			}
		}

		private string PreviousButtonText
		{
			get
			{
				var stepType = GetEffectiveStepType(ActiveStepIndex);
				return stepType == WizardStepType.Finish ? FinishPreviousButtonText : StepPreviousButtonText;
			}
		}

		#endregion

		#region Navigation Methods

		private async Task HandleNextClick()
		{
			var nextIndex = ActiveStepIndex + 1;
			var args = new WizardNavigationEventArgs(ActiveStepIndex, nextIndex);
			await OnNextButtonClick.InvokeAsync(args);

			if (!args.Cancel && nextIndex < _steps.Count)
			{
				ActiveStepIndex = nextIndex;
				await ActiveStepIndexChanged.InvokeAsync(ActiveStepIndex);
				await OnActiveStepChanged.InvokeAsync(EventArgs.Empty);
			}
		}

		private async Task HandlePreviousClick()
		{
			var prevIndex = ActiveStepIndex - 1;

			// Skip steps where AllowReturn is false
			while (prevIndex >= 0 && !_steps[prevIndex].AllowReturn)
			{
				prevIndex--;
			}

			if (prevIndex < 0) return;

			var args = new WizardNavigationEventArgs(ActiveStepIndex, prevIndex);
			await OnPreviousButtonClick.InvokeAsync(args);

			if (!args.Cancel)
			{
				ActiveStepIndex = prevIndex;
				await ActiveStepIndexChanged.InvokeAsync(ActiveStepIndex);
				await OnActiveStepChanged.InvokeAsync(EventArgs.Empty);
			}
		}

		private async Task HandleFinishClick()
		{
			var nextIndex = ActiveStepIndex + 1;
			var args = new WizardNavigationEventArgs(ActiveStepIndex, nextIndex);
			await OnFinishButtonClick.InvokeAsync(args);

			if (!args.Cancel)
			{
				if (!string.IsNullOrEmpty(FinishDestinationPageUrl))
				{
					NavigationManager.NavigateTo(FinishDestinationPageUrl);
				}
				else if (nextIndex < _steps.Count && GetEffectiveStepType(nextIndex) == WizardStepType.Complete)
				{
					ActiveStepIndex = nextIndex;
					await ActiveStepIndexChanged.InvokeAsync(ActiveStepIndex);
					await OnActiveStepChanged.InvokeAsync(EventArgs.Empty);
				}
			}
		}

		private async Task HandleCancelClick()
		{
			await OnCancelButtonClick.InvokeAsync(EventArgs.Empty);
			if (!string.IsNullOrEmpty(CancelDestinationPageUrl))
			{
				NavigationManager.NavigateTo(CancelDestinationPageUrl);
			}
		}

		private async Task HandleSideBarNavigation(int stepIndex)
		{
			if (stepIndex < 0 || stepIndex >= _steps.Count) return;
			if (!_steps[stepIndex].AllowReturn && stepIndex < ActiveStepIndex) return;

			var args = new WizardNavigationEventArgs(ActiveStepIndex, stepIndex);
			await OnSideBarButtonClick.InvokeAsync(args);

			if (!args.Cancel)
			{
				ActiveStepIndex = stepIndex;
				await ActiveStepIndexChanged.InvokeAsync(ActiveStepIndex);
				await OnActiveStepChanged.InvokeAsync(EventArgs.Empty);
			}
		}

		#endregion

		#region Style Helpers

		private string GetSideBarStyleString()
		{
			return SideBarStyle != null ? SideBarStyle.ToString() : null;
		}

		private string GetSideBarStyleClass()
		{
			return SideBarStyle?.CssClass;
		}

		private string GetHeaderStyleString()
		{
			return HeaderStyle != null ? HeaderStyle.ToString() : null;
		}

		private string GetHeaderStyleClass()
		{
			return HeaderStyle?.CssClass;
		}

		private string GetStepStyleString()
		{
			return StepStyle != null ? StepStyle.ToString() : null;
		}

		private string GetStepStyleClass()
		{
			return StepStyle?.CssClass;
		}

		private string GetNavigationStyleString()
		{
			return NavigationStyle != null ? NavigationStyle.ToString() : null;
		}

		private string GetNavigationStyleClass()
		{
			return NavigationStyle?.CssClass;
		}

		private string GetNavigationButtonStyleString()
		{
			return NavigationButtonStyle != null ? NavigationButtonStyle.ToString() : null;
		}

		private string GetNavigationButtonStyleClass()
		{
			return NavigationButtonStyle?.CssClass;
		}

		#endregion

	}
}
