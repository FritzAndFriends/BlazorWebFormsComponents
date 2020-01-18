using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class TextBoxStyle : UiStyle
	{
		[CascadingParameter(Name = "TextBoxStyle")]
		protected Style theTextBoxStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
