using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
	public partial class AspNetValidationSummary : BaseStyledComponent, IDisposable
	{
		private EditContext _previousEditContext;
		private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;

		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ValidationSummaryDisplayMode DisplayMode { get; set; } = ValidationSummaryDisplayMode.BulletList;

		[Parameter] public string HeaderText { get; set; }

		[Parameter] public bool ShowSummary { get; set; } = true;

		[Parameter] public string ValidationGroup { get; set; }

		public bool IsValid => FilteredMessages.Any();

		public IEnumerable<string> ValidationMessages => FilteredMessages.Select(x =>
		{
			var fieldMsg = x.Split('\x1F')[0];
			var commaIndex = fieldMsg.IndexOf(',');
			return commaIndex >= 0 ? fieldMsg.Substring(commaIndex + 1) : fieldMsg;
		});

		private IEnumerable<string> FilteredMessages
		{
			get
			{
				var messages = CurrentEditContext.GetValidationMessages();
				if (!string.IsNullOrEmpty(ValidationGroup))
				{
					messages = messages.Where(x =>
					{
						var parts = x.Split('\x1F');
						return parts.Length > 1 && parts[1] == ValidationGroup;
					});
				}
				return messages;
			}
		}

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
