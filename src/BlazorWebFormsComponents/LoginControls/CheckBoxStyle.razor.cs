using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class CheckBoxStyle : UiStyle
	{
		[CascadingParameter(Name = "CheckBoxStyle")]
		protected TableItemStyle theCheckBoxStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
