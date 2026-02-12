using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class SuccessTextStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "SuccessTextStyle")]
		protected TableItemStyle theSuccessTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
