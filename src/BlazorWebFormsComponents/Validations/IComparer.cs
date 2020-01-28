namespace BlazorWebFormsComponents.Validations
{
	public interface IComparer
	{
		bool TryConvert(string text, bool cultureInvariant, out object value);
		int CompareTo(object value, object valueToCompare);

	}
}
