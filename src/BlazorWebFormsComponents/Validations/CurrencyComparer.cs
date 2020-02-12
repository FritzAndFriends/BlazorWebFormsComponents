using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Validations
{
	public class CurrencyComparer : IComparer
	{
		public bool TryConvert(string text, bool cultureInvariant, out object value)
		{
			string cleanInput;
			if (cultureInvariant)
			{

				cleanInput = ConvertCurrency(text, CultureInfo.InvariantCulture.NumberFormat);

			}
			else
			{

				cleanInput = ConvertCurrency(text, NumberFormatInfo.CurrentInfo);

			}

			if (cleanInput != null)
			{

				value = decimal.Parse(cleanInput, CultureInfo.InvariantCulture);
				return true;

			}
			else
			{

				value = default;
				return false;

			}
		}

		private static string ConvertCurrency(string text, NumberFormatInfo info)
		{

			var decimalChar = info.CurrencyDecimalSeparator;
			var groupChar = info.CurrencyGroupSeparator;

			// VSWhidbey 83165
			string beginGroupSize, subsequentGroupSize;
			var groupSize = GetCurrencyGroupSize(info);
			if (groupSize > 0)
			{
				var groupSizeText = groupSize.ToString(NumberFormatInfo.InvariantInfo);
				beginGroupSize = "{1," + groupSizeText + "}";
				subsequentGroupSize = "{" + groupSizeText + "}";
			}
			else
			{
				beginGroupSize = subsequentGroupSize = "+";
			}

			// Map non-break space onto regular space for parsing
			if (groupChar[0] == 160)
				groupChar = " ";
			var digits = info.CurrencyDecimalDigits;
			var hasDigits = (digits > 0);
			var currencyExpression =
					"^\\s*([-\\+])?((\\d" + beginGroupSize + "(\\" + groupChar + "\\d" + subsequentGroupSize + ")+)|\\d*)"
					+ (hasDigits ? "\\" + decimalChar + "?(\\d{0," + digits.ToString(NumberFormatInfo.InvariantInfo) + "})" : string.Empty)
					+ "\\s*$";

			var m = Regex.Match(text, currencyExpression);
			if (!m.Success)
			{
				return null;
			}

			// Make sure there are some valid digits
			if (m.Groups[2].Length == 0 && hasDigits && m.Groups[5].Length == 0)
			{
				return null;
			}

			return m.Groups[1].Value
						 + m.Groups[2].Value.Replace(groupChar, string.Empty)
						 + ((hasDigits && m.Groups[5].Length > 0) ? "." + m.Groups[5].Value : string.Empty);
		}

		private static int GetCurrencyGroupSize(NumberFormatInfo info)
		{

			var groupSizes = info.CurrencyGroupSizes;
			if (groupSizes != null && groupSizes.Length == 1)
			{

				return groupSizes[0];

			}
			else
			{

				return -1;

			}

		}

		public int CompareTo(object value, object valueToCompare)
		{

			return ((decimal)value).CompareTo(valueToCompare);

		}
	}
}
