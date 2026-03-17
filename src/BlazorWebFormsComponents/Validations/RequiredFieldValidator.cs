using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
	public partial class RequiredFieldValidator<Type> : BaseValidator<Type>
	{
		/// <summary>
		/// Gets or sets the initial value of the associated input control.
		/// Validation fails if the control's value matches this initial value.
		/// </summary>
		[Parameter] public string InitialValue { get; set; } = "";

		public override bool Validate(string value)
		{
			return (value?.Trim() ?? "") != (InitialValue?.Trim() ?? "");
		}
	}
}
