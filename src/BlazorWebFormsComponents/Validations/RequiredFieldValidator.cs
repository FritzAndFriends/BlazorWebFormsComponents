namespace BlazorWebFormsComponents.Validations
{
	public partial class RequiredFieldValidator<Type> : BaseValidator<Type>
	{
		public override bool Validate(string value)
		{
			return !string.IsNullOrWhiteSpace(value);
		}
	}
}
