namespace BlazorWebFormsComponents.Validations.TypeComparers
{
	public interface IComparer
	{
		bool TryConvert(string text, bool cultureInvariant, out object value);
		int CompareTo(object value, object valueToCompare);

	}
}
