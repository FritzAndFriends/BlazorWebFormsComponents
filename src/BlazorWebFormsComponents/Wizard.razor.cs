using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class Wizard : BaseStyledComponent
	{

		#region SSR Form Navigation

		private const string WizardActionFieldName = "__wizard_action";
		private const string WizardStepFieldName = "__wizard_step";
		private bool _formProcessed;

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			// In SSR mode, restore step index from form hidden field on postback
			if (CurrentRenderMode == WebFormsRenderMode.StaticSSR)
			{
				var httpContext = HttpContextAccessor?.HttpContext;
				if (httpContext != null && HttpMethods.IsPost(httpContext.Request.Method)
					&& httpContext.Request.HasFormContentType)
				{
					var form = await httpContext.Request.ReadFormAsync();

					// Restore the step index from hidden field
					if (int.TryParse(form[WizardStepFieldName], out var savedStep))
					{
						ActiveStepIndex = savedStep;
					}

					// Determine navigation action from button click
					var action = form[WizardActionFieldName].ToString();
					if (!string.IsNullOrEmpty(action))
					{
						_pendingAction = action;
					}
				}
			}
		}

		private string _pendingAction;

		/// <summary>
		/// Called after steps have registered to process any pending SSR navigation action.
		/// </summary>
		internal void ProcessPendingNavigation()
		{
			if (_formProcessed || string.IsNullOrEmpty(_pendingAction)) return;
			// Wait until we have enough steps registered to navigate
			// (navigation target must be within known step bounds)
			var action = _pendingAction;

			if (action == StartNextButtonText || action == StepNextButtonText)
			{
				var nextIndex = ActiveStepIndex + 1;
				if (nextIndex >= _steps.Count) return; // Not enough steps registered yet
				_formProcessed = true;
				_pendingAction = null;
				ActiveStepIndex = nextIndex;
			}
			else if (action == StepPreviousButtonText || action == FinishPreviousButtonText)
			{
				_formProcessed = true;
				_pendingAction = null;
				var prevIndex = ActiveStepIndex - 1;
				while (prevIndex >= 0 && !_steps[prevIndex].AllowReturn)
				{
					prevIndex--;
				}
				if (prevIndex >= 0)
				{
					ActiveStepIndex = prevIndex;
				}
			}
			else if (action == FinishButtonText || action == FinishCompleteButtonText)
			{
				var nextIndex = ActiveStepIndex + 1;
				if (nextIndex >= _steps.Count) return; // Not enough steps registered yet
				_formProcessed = true;
				_pendingAction = null;
				if (!string.IsNullOrEmpty(FinishDestinationPageUrl))
				{
					NavigationManager.NavigateTo(FinishDestinationPageUrl);
				}
				else if (GetEffectiveStepType(nextIndex) == WizardStepType.Complete)
				{
					ActiveStepIndex = nextIndex;
				}
			}
			else if (action == CancelButtonText)
			{
				_formProcessed = true;
				_pendingAction = null;
				if (!string.IsNullOrEmpty(CancelDestinationPageUrl))
				{
					NavigationManager.NavigateTo(CancelDestinationPageUrl);
				}
			}
			else
			{
				_formProcessed = true;
				_pendingAction = null;
			}
		}

		#endregion

		#region Steps

		private List<WizardStep> _steps = new();

		public IReadOnlyList<WizardStep> WizardStepsList => _steps;

		internal void AddStep(WizardStep step)
		{
			if (!_steps.Contains(step))
			{
				_steps.Add(step);
				ProcessPendingNavigation();
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

		private WizardStepType CurrentStepType => GetEffectiveStepType(ActiveStepIndex);

		private bool ShowPreviousButton
		{
			get
			{
				return CurrentStepType == WizardStepType.Step || CurrentStepType == WizardStepType.Finish;
			}
		}

		private bool ShowNextButton
		{
			get
			{
				return CurrentStepType == WizardStepType.Start || CurrentStepType == WizardStepType.Step;
			}
		}

		private bool ShowFinishButton
		{
			get
			{
				return CurrentStepType == WizardStepType.Finish;
			}
		}

		private bool IsCompleteStep
		{
			get
			{
				return CurrentStepType == WizardStepType.Complete;
			}
		}

		private RenderFragment CurrentNavigationTemplate => CurrentStepType switch
		{
			WizardStepType.Start => StartNavigationTemplate,
			WizardStepType.Step => StepNavigationTemplate,
			WizardStepType.Finish => FinishNavigationTemplate,
			_ => null
		};

		private string NextButtonText
		{
			get
			{
				return CurrentStepType == WizardStepType.Start ? StartNextButtonText : StepNextButtonText;
			}
		}

		private string PreviousButtonText
		{
			get
			{
				return CurrentStepType == WizardStepType.Finish ? FinishPreviousButtonText : StepPreviousButtonText;
			}
		}

		private string CalculatedFinishButtonText
		{
			get
			{
				if (!string.IsNullOrEmpty(FinishCompleteButtonText) && !string.Equals(FinishCompleteButtonText, "Finish", StringComparison.Ordinal))
				{
					return FinishCompleteButtonText;
				}

				return !string.IsNullOrEmpty(FinishButtonText) ? FinishButtonText : "Finish";
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
