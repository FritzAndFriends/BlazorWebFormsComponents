using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
  public abstract class BaseDataBindingComponent : BaseWebFormsComponent
  {

	#region Data Binding Events

	[Parameter]
	public EventCallback<EventArgs> OnDataBinding { get; set; }

	[Parameter]
	public EventCallback<EventArgs> OnDataBound { get; set; }

	[Parameter]
	public EventCallback<ListViewItemEventArgs> OnItemDataBound { get; set; }

	#endregion

  }

}
