using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginButtonStyle : UiStyle
	{
		[CascadingParameter(Name = "LoginButtonStyle")]
		protected Style theLoginButtonStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
