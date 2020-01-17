using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LabelStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "LabelStyle")]
		protected TableItemStyle theLabelStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
