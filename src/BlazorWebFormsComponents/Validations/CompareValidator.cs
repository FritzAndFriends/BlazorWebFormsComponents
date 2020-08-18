using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
  public class CompareValidator<InputType> : BaseCompareValidator<InputType>
  {

	[Parameter] public ValidationCompareOperator Operator { get; set; } = ValidationCompareOperator.Equal;

	[Parameter] public string ValueToCompare { get; set; }

	public override bool Validate(string value)
	{

	  if (value is null)
	  {

		return true;

	  }

	  if (value.Trim().Length == 0)
	  {

		return true;

	  }

	  return Compare(value, false, ValueToCompare, CultureInvariantValues, Operator, Type);

	}

  }

}
