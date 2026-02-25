using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents.Validations
{
	public abstract partial class BaseValidator<Type> : BaseStyledComponent, IValidationGroupMember
	{
		// BANG used because we know it will set during OnInitialized and thus no need to worry about null
		private ValidationMessageStore _messageStore = default!;
		protected bool IsValid { get; set; } = true;
		private bool _validationRequested = false;

		[CascadingParameter] EditContext CurrentEditContext { get; set; }
		[CascadingParameter(Name = "ValidationGroupCoordinator")] 
		ValidationGroupCoordinator Coordinator { get; set; }
		/// <summary>
		/// String ID of the target control, matching the Web Forms ControlToValidate pattern.
		/// Maps to a property name on the EditContext model. Use this for migration from Web Forms.
		/// </summary>
		[Parameter] public string ControlToValidate { get; set; }
		/// <summary>
		/// Blazor-native alternative: a ForwardRef to the InputBase control to validate.
		/// When both ControlToValidate and ControlRef are set, ControlRef takes precedence.
		/// </summary>
		[Parameter] public ForwardRef<InputBase<Type>> ControlRef { get; set; }
		[Parameter] public string Text { get; set; }
		[Parameter] public string ErrorMessage { get; set; }
		[Parameter] public string ValidationGroup { get; set; }
		[Parameter] public HorizontalAlign HorizontalAlign { get; set; }
		[Parameter] public VerticalAlign VerticalAlign { get; set; }
		[Parameter] public ValidatorDisplay Display { get; set; } = ValidatorDisplay.Static;
		[Parameter] public bool SetFocusOnError { get; set; }

		public abstract bool Validate(string value);

		protected string DisplayStyle => Display switch
		{
			ValidatorDisplay.None => "display:none;",
			ValidatorDisplay.Dynamic when IsValid => "display:none;",
			ValidatorDisplay.Static when IsValid => "visibility:hidden;",
			_ => ""
		};

		protected StyleBuilder CalculatedStyle => this.ToStyle();

		protected override void OnInitialized()
		{
			_messageStore = new ValidationMessageStore(CurrentEditContext);

			CurrentEditContext.OnValidationRequested += EventHandler;

			// Register with validation group coordinator if available
			if (Coordinator != null)
			{
				Coordinator.RegisterValidator(this);
			}

			this.SetFontsFromAttributes(AdditionalAttributes);

			base.OnInitialized();
		}

		protected override ValueTask Dispose(bool disposing)
		{
			// Unsubscribe to avoid memory leaks.
			if (CurrentEditContext != null)
			{
				CurrentEditContext.OnValidationRequested -= EventHandler;
			}

			// Unregister from validation group coordinator
			if (Coordinator != null)
			{
				Coordinator.UnregisterValidator(this);
			}

			return base.Dispose(disposing);
		}

		private void EventHandler(object sender, ValidationRequestedEventArgs eventArgs)
		{
			// If we have a coordinator and validation wasn't explicitly requested for this validator,
			// skip validation - it will be handled by group-based validation
			if (Coordinator != null && !_validationRequested)
			{
				return;
			}

			_validationRequested = false; // Reset for next validation

			var name = GetFieldName();
			var fieldIdentifier = CurrentEditContext.Field(name);

			_messageStore.Clear(fieldIdentifier);
			var value = GetCurrentValueAsString(name);

			if (!Enabled || Validate(value))
			{
				IsValid = true;
			}
			else
			{
				IsValid = false;
				// Text is for validator, ErrorMessage is for validation summary
				_messageStore.Add(fieldIdentifier, Text + "," + ErrorMessage + "\x1F" + (ValidationGroup ?? ""));

				if (SetFocusOnError)
				{
					_ = JsRuntime.InvokeVoidAsync("bwfc.Validation.SetFocus", fieldIdentifier.FieldName);
				}
			}

			CurrentEditContext.NotifyValidationStateChanged();
		}

		/// <summary>
		/// Resolves the field name from either ControlRef (ForwardRef) or ControlToValidate (string ID).
		/// ControlRef takes precedence when both are set.
		/// </summary>
		private string GetFieldName()
		{
			if (ControlRef?.Current != null)
			{
				if (ControlRef.Current.ValueExpression.Body is MemberExpression memberExpression)
				{
					return memberExpression.Member.Name;
				}

				throw new InvalidOperationException("You should not have seen this message, but now that you do" +
					"I want you to open an issue here https://github.com/fritzAndFriends/BlazorWebFormsComponents/issues " +
					"with a title 'ValueExpression.Body is not MemberExpression' and a sample code to reproduce this. Thanks!");
			}

			if (!string.IsNullOrEmpty(ControlToValidate))
			{
				return ControlToValidate;
			}

			throw new InvalidOperationException(
				"Either ControlToValidate (string ID) or ControlRef (ForwardRef) must be set on the validator.");
		}

		private string GetCurrentValueAsString(string fieldName)
		{
			// When a ForwardRef is available, use the InputBase's internal value
			if (ControlRef?.Current != null)
			{
				var getter = typeof(InputBase<Type>).GetProperty("CurrentValueAsString", BindingFlags.NonPublic | BindingFlags.Instance);
				return getter.GetValue(ControlRef.Current) as string;
			}

			// When using string ControlToValidate, resolve via the EditContext model
			var model = CurrentEditContext.Model;
			var property = model.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
			if (property != null)
			{
				var value = property.GetValue(model);
				return value?.ToString();
			}

			// Try fields as well (for model classes with public fields)
			var field = model.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
			if (field != null)
			{
				var value = field.GetValue(model);
				return value?.ToString();
			}

			return null;
		}

		/// <summary>
		/// Implementation of IValidationGroupMember - called by ValidationGroupCoordinator
		/// </summary>
		public void PerformValidation()
		{
			_validationRequested = true;
			// Trigger validation through the EditContext
			CurrentEditContext.Validate();
		}
	}
}
