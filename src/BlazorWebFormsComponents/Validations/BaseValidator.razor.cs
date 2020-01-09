using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebFormsComponents.Validations
{
	public abstract partial class BaseValidator<Type> : BaseWebFormsComponent
	{
		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ForwardRef<InputBase<Type>> ControlToValidate { get; set; }
		[Parameter] public string Text { get; set; }
		[Parameter] public string ErrorMessage { get; set; }

		public bool IsValid { get; set; } = true;

		protected override void OnInitialized()
		{
			// ValidationMessageStore is sealed. I have a feeling I will need to implement my own.
			var messages = new ValidationMessageStore(CurrentEditContext);

			CurrentEditContext.OnValidationRequested += (sender, eventArgs) => EventHandler((EditContext)sender, messages);

			base.OnInitialized();
		}

		public void EventHandler(EditContext editContext, ValidationMessageStore messages)
		{
			string name;
			if (ControlToValidate.Current.ValueExpression.Body is MemberExpression memberExpression)
			{
				name = memberExpression.Member.Name;
			}
			else
			{
				throw new Exception();
			}

			var fieldIdentifier = CurrentEditContext.Field(name);

			// TODO : The field could have multiple validator
			// I Have to find a way to clear message only for this validator.
			messages.Clear(fieldIdentifier);

			var value = typeof(InputBase<Type>).GetProperty("CurrentValueAsString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ControlToValidate.Current) as string;

			if (Validate(value))
			{
				IsValid = true;
			}
			else
			{
				IsValid = false;
				// Text is for validator,ErrorMessage is for validation summary
				messages.Add(fieldIdentifier, Text + "," + ErrorMessage);
			}

			editContext.NotifyValidationStateChanged();
		}

		public abstract bool Validate(string value);
	}
}
