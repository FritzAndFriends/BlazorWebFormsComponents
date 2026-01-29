using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

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
		[Parameter] public ForwardRef<InputBase<Type>> ControlToValidate { get; set; }
		[Parameter] public string Text { get; set; }
		[Parameter] public string ErrorMessage { get; set; }
		[Parameter] public string ValidationGroup { get; set; }
		[Parameter] public HorizontalAlign HorizontalAlign { get; set; }
		[Parameter] public VerticalAlign VerticalAlign { get; set; }

		public abstract bool Validate(string value);

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

			string name;
			if (ControlToValidate.Current.ValueExpression.Body is MemberExpression memberExpression)
			{
				name = memberExpression.Member.Name;
			}
			else
			{
				throw new InvalidOperationException("You should not have seen this message, but now that you do" +
					"I want you to open an issue here https://github.com/fritzAndFriends/BlazorWebFormsComponents/issues " +
					"with a title 'ValueExpression.Body is not MemberExpression' and a sample code to reproduce this. Thanks!");
			}

			var fieldIdentifier = CurrentEditContext.Field(name);

			_messageStore.Clear(fieldIdentifier);
			var value = GetCurrentValueAsString();

			if (!Enabled || Validate(value))
			{
				IsValid = true;
			}
			else
			{
				IsValid = false;
				// Text is for validator, ErrorMessage is for validation summary
				_messageStore.Add(fieldIdentifier, Text + "," + ErrorMessage);
			}

			CurrentEditContext.NotifyValidationStateChanged();
		}

		private string GetCurrentValueAsString()
		{
			// The getter variable could be stored in a static
			// readonly variable to lessen the perf impact of reflection.
			var getter = typeof(InputBase<Type>).GetProperty("CurrentValueAsString", BindingFlags.NonPublic | BindingFlags.Instance);
			return getter.GetValue(ControlToValidate.Current) as string;
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
