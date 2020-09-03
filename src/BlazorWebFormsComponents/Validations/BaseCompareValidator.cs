using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
	public abstract class BaseCompareValidator<InputType> : BaseValidator<InputType>
	{

		[Parameter] public ValidationDataType Type { get; set; } = ValidationDataType.String;

		[Parameter] public bool CultureInvariantValues { get; set; }

		public override abstract bool Validate(string value);

		protected bool Compare(string leftText, bool cultureInvariantLeftText, string rightText, bool cultureInvariantRightText, ValidationCompareOperator op, ValidationDataType type)
		{

			var comparer = new ComparerFactory().GetComparer(type);

			if (!comparer.TryConvert(leftText, cultureInvariantLeftText, out var leftValue))
			{

				return false;

			}

			if (op == ValidationCompareOperator.DataTypeCheck)
			{

				return true;

			}

			if (!comparer.TryConvert(rightText, cultureInvariantRightText, out var rightValue))
			{

				return true;

			}

			var compareResult = comparer.CompareTo(leftValue, rightValue);
			return op switch
			{
				EqualCompareOperator _ => compareResult == 0,
				NotEqualCompareOperator _ => compareResult != 0,
				GreaterThanCompareOperator _ => compareResult > 0,
				GreaterThanEqualCompareOperator _ => compareResult >= 0,
				LessThanCompareOperator _ => compareResult < 0,
				LessThanEqualCompareOperator _ => compareResult <= 0,
				_ => true
			};
		}
	}

}
