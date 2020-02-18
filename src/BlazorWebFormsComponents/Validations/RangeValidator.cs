using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
	public class RangeValidator<InputType> : BaseCompareValidator<InputType>
	{

		[Parameter] public string MaximumValue { get; set; }

		[Parameter] public string MinimumValue { get; set; }


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

			return Compare(value, false, MinimumValue, CultureInvariantValues, ValidationCompareOperator.GreaterThanEqual, Type) &&
							Compare(value, false, MaximumValue, CultureInvariantValues, ValidationCompareOperator.LessThanEqual, Type);

		}

	}

}
