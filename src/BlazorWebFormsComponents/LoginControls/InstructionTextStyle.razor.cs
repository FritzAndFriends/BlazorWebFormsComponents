using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class InstructionTextStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "InstructionTextStyle")]
		protected TableItemStyle theInstructionTextStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
