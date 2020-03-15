using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
  public partial class Label : BaseWebFormsComponent
	{ 
		[Parameter] public object Text { get; set; }

	}
}
