using System.Globalization;

namespace BlazorWebFormsComponents.Validations
{
	public class IntegerComparer : IComparer
	{
		public bool TryConvert(string text, bool cultureInvariant, out object value)
		{

			var tried = int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number);
			value = number;
			return tried;

		}

		public int CompareTo(object value, object valueToCompare)
		{

			return ((int)value).CompareTo(valueToCompare);

		}
	}
}
