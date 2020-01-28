using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
	public class CompareValidator<InputType> : BaseValidator<InputType>
	{

		[Parameter] public ValidationDataType Type { get; set; } = ValidationDataType.String;

		[Parameter] public ValidationCompareOperator Operator { get; set; } = ValidationCompareOperator.Equal;

		[Parameter] public string ValueToCompare { get; set; }

		[Parameter] public bool CultureInvariantValues { get; set; }

		public override bool Validate(string value)
		{

			var comparer = new ComparerFactory().GetComparer(Type);

			if (!comparer.TryConvert(value, false, out var convertedValue))
			{

				return false;

			}

			if (Operator == ValidationCompareOperator.DataTypeCheck)
			{

				return true;

			}

			if (!comparer.TryConvert(ValueToCompare, CultureInvariantValues, out var valueToCompare))
			{

				return true;

			}

			var compareResult = comparer.CompareTo(convertedValue, valueToCompare);

			return Operator switch
			{
				EqualCompareOperator e => compareResult == 0,
				NotEqualCompareOperator e => compareResult != 0,
				GreaterThanCompareOperator g => compareResult > 0,
				GreaterThanEqualCompareOperator g => compareResult >= 0,
				LessThanCompareOperator l => compareResult < 0,
				LessThanEqualCompareOperator l => compareResult <= 0,
				_ => true
			};

		}
	}
}
