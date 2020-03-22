using Microsoft.AspNetCore.Components;
namespace BlazorWebFormsComponents {	public partial class Button : ButtonBaseComponent
	{
		[Parameter]
		public string OnClientClick { get; set; }
	}
}
