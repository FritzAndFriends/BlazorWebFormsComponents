using System;
using System.Collections.Generic;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents.Validations
{
	public class ComparerFactory
	{

		private readonly static Dictionary<ValidationDataType, IComparer> _comparers = new Dictionary<ValidationDataType, IComparer>();

		static ComparerFactory()
		{

			_comparers.Add(ValidationDataType.String, new StringComparer());
			_comparers.Add(ValidationDataType.Integer, new IntegerComparer());
			_comparers.Add(ValidationDataType.Double, new DoubleComparer());
			_comparers.Add(ValidationDataType.Date, new DateComparer());
			_comparers.Add(ValidationDataType.Currency, new CurrencyComparer());

		}

		public IComparer GetComparer(ValidationDataType dataType)
		{

			if (_comparers.TryGetValue(dataType, out var comparer))
			{

				return comparer;

			}
			else
			{

				throw new ArgumentOutOfRangeException(nameof(dataType));

			}

		}
	}
}
