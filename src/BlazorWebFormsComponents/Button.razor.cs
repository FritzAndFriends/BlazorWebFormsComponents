namespace BlazorWebFormsComponents
{
	public partial class Button : ButtonBaseComponent
	{
		internal string CalculatedButtonType => CausesValidation ? "submit" : "button";
	}
}
