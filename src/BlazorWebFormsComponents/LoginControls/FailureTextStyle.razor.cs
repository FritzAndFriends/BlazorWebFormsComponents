using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class FailureTextStyle : UiStyle
	{
		[CascadingParameter(Name = "FailureTextStyle")]
		protected TableItemStyle theFailureTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
