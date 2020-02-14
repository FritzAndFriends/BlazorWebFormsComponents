using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Validations
{
	public class DoubleComparer : IComparer
	{
		public bool TryConvert(string text, bool cultureInvariant, out object value)
		{
			string cleanInput;
			if (cultureInvariant)
			{

				cleanInput = ConvertDouble(text, CultureInfo.InvariantCulture.NumberFormat);

			}
			else
			{

				cleanInput = ConvertDouble(text, NumberFormatInfo.CurrentInfo);

			}

			if (cleanInput != null)
			{

				var tried = double.TryParse(cleanInput, NumberStyles.None, CultureInfo.InvariantCulture, out var number);
				value = number;
				return tried;

			}
			else
			{

				value = default;
				return false;

			}
		}

		private static string ConvertDouble(string text, NumberFormatInfo info)
		{
			// VSWhidbey 83156: If text is empty, it would be default to 0 for
			// backward compatibility reason.
			if (text.Length == 0)
			{

				return "0";

			}

			var decimalChar = info.NumberDecimalSeparator;
			var doubleExpression = "^\\s*([-\\+])?(\\d*)\\" + decimalChar + "?(\\d*)\\s*$";

			var m = Regex.Match(text, doubleExpression);
			if (!m.Success)
			{

				return null;

			}

			// Make sure there are some valid digits
			if (m.Groups[2].Length == 0 && m.Groups[3].Length == 0)
			{

				return null;

			}

			return m.Groups[1].Value
						 + (m.Groups[2].Length > 0 ? m.Groups[2].Value : "0")
						 + ((m.Groups[3].Length > 0) ? "." + m.Groups[3].Value : string.Empty);
		}

		public int CompareTo(object value, object valueToCompare)
		{

			return ((double)value).CompareTo(valueToCompare);

		}
	}
}
