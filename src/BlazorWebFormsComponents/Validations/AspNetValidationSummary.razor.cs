using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
	public partial class AspNetValidationSummary : Component, IDisposable
	{
		private EditContext _previousEditContext;
		private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;

		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ValidationSummaryDisplayMode DisplayMode { get; set; } = ValidationSummaryDisplayMode.BulletList;

		public bool IsValid => CurrentEditContext.GetValidationMessages().Any();

		public IEnumerable<string> ValidationMessages => CurrentEditContext.GetValidationMessages().Select(x => x.Split(',')[1]);

		public AspNetValidationSummary()
		{
			_validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
		}

		protected override void OnParametersSet()
		{

			if (CurrentEditContext == null)
			{
				throw new InvalidOperationException($"{nameof(ValidationSummary)} requires a cascading parameter " +
						$"of type {nameof(EditContext)}. For example, you can use {nameof(ValidationSummary)} inside " +
						$"an {nameof(EditForm)}.");
			}

			if (CurrentEditContext != _previousEditContext)
			{
				DetachValidationStateChangedListener();
				CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
				_previousEditContext = CurrentEditContext;
			}
		}

		protected override void OnInitialized()
		{

			this.SetFontsFromAttributes(AdditionalAttributes);

			base.OnInitialized();

		}

		public void Dispose()
		{
			DetachValidationStateChangedListener();
		}

		private void DetachValidationStateChangedListener()
		{
			if (_previousEditContext != null)
			{
				_previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
			}
		}
	}
}
