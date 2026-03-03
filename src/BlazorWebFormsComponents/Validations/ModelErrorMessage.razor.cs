using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
	/// <summary>
	/// Displays model state error messages for a specific key, matching the
	/// ASP.NET Web Forms <c>&lt;asp:ModelErrorMessage&gt;</c> control.
	/// Renders a <c>&lt;span&gt;</c> with the error text when errors exist,
	/// and nothing when there are no errors for the given key.
	/// </summary>
	public partial class ModelErrorMessage : BaseStyledComponent, IDisposable
	{
		private EditContext _previousEditContext;
		private bool _hasErrors;
		private string _errorHtml = string.Empty;

		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		/// <summary>
		/// The key in the model state / EditContext to display errors for.
		/// Maps to a field name on the EditContext model.
		/// </summary>
		[Parameter] public string ModelStateKey { get; set; }

		/// <summary>
		/// The ID of the associated input control. Used with
		/// <see cref="SetFocusOnError"/> to focus the control when an error is displayed.
		/// </summary>
		[Parameter] public string AssociatedControlID { get; set; }

		/// <summary>
		/// When true, focuses the associated control when an error is displayed.
		/// Requires <see cref="AssociatedControlID"/> to be set.
		/// </summary>
		[Parameter] public bool SetFocusOnError { get; set; }

		protected override void OnParametersSet()
		{
			if (CurrentEditContext == null)
			{
				throw new InvalidOperationException(
					$"{nameof(ModelErrorMessage)} requires a cascading parameter of type " +
					$"{nameof(EditContext)}. Use {nameof(ModelErrorMessage)} inside an EditForm.");
			}

			if (CurrentEditContext != _previousEditContext)
			{
				DetachListener();
				CurrentEditContext.OnValidationStateChanged += OnValidationStateChanged;
				_previousEditContext = CurrentEditContext;
			}

			UpdateErrorState();
			base.OnParametersSet();
		}

		protected override void OnInitialized()
		{
			this.SetFontsFromAttributes(AdditionalAttributes);
			base.OnInitialized();
		}

		private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
		{
			UpdateErrorState();
			InvokeAsync(StateHasChanged);
		}

		private void UpdateErrorState()
		{
			if (string.IsNullOrEmpty(ModelStateKey) || CurrentEditContext == null)
			{
				_hasErrors = false;
				_errorHtml = string.Empty;
				return;
			}

			var field = CurrentEditContext.Field(ModelStateKey);
			var messages = CurrentEditContext.GetValidationMessages(field)
				.Select(StripValidatorMetadata)
				.Where(m => !string.IsNullOrEmpty(m))
				.ToList();

			_hasErrors = messages.Count > 0;
			_errorHtml = _hasErrors
				? string.Join("<br>", messages.Select(System.Net.WebUtility.HtmlEncode))
				: string.Empty;

			if (_hasErrors && SetFocusOnError && !string.IsNullOrEmpty(AssociatedControlID))
			{
				_ = JsRuntime.InvokeVoidAsync("bwfc.Validation.SetFocus", AssociatedControlID);
			}
		}

		private string GetCssClass()
		{
			return string.IsNullOrEmpty(CssClass) ? null : CssClass;
		}

		/// <summary>
		/// BWFC validators encode messages as "Text,ErrorMessage\x1FValidationGroup".
		/// This method strips the metadata to extract just the display-friendly error message.
		/// Messages added directly via <see cref="ValidationMessageStore"/> are passed through as-is.
		/// </summary>
		private static string StripValidatorMetadata(string raw)
		{
			// Strip validation group suffix
			var groupSep = raw.IndexOf('\x1F');
			var fieldMsg = groupSep >= 0 ? raw.Substring(0, groupSep) : raw;

			// If it contains the Text,ErrorMessage pattern, take the ErrorMessage part
			var commaIndex = fieldMsg.IndexOf(',');
			if (commaIndex >= 0)
			{
				return fieldMsg.Substring(commaIndex + 1);
			}

			return fieldMsg;
		}

		public void Dispose()
		{
			DetachListener();
		}

		private void DetachListener()
		{
			if (_previousEditContext != null)
			{
				_previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
			}
		}
	}
}
