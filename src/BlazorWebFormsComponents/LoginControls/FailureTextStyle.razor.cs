using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class FailureTextStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "FailureTextStyle")]
		protected TableItemStyle theFailureTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
