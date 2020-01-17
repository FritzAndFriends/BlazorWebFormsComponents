using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class ValidatorTextStyle : UiStyle
	{
		[CascadingParameter(Name = "ValidatorTextStyle")]
		protected Style theValidatorTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
