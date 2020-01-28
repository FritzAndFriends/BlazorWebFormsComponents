using System.Globalization;

namespace BlazorWebFormsComponents.Validations
{
	public class StringComparer : IComparer
	{
		public bool TryConvert(string text, bool cultureInvariant, out object value)
		{

			value = text;
			return true;

		}

		public int CompareTo(object value, object valueToCompare)
		{

			return string.Compare((string)value, (string)valueToCompare, false, CultureInfo.CurrentCulture);

		}
	}
}
