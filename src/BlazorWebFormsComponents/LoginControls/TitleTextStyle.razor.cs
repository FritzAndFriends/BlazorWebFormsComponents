using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class TitleTextStyle : UiStyle
	{
		[CascadingParameter(Name = "TitleTextStyle")]
		protected TableItemStyle theTitleTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
