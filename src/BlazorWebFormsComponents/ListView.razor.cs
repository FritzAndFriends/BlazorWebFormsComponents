using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlazorWebFormsComponents
{

  public partial class ListView<ItemType> : BaseModelBindingComponent<ItemType>
  {

	public ListView()
	{
	}

	#region Templates
	[Parameter] public RenderFragment EmptyDataTemplate { get; set; }
	[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }
	[Parameter] public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }
	[Parameter] public RenderFragment ItemSeparatorTemplate { get; set; }
	[Parameter] public RenderFragment GroupSeparatorTemplate { get; set; }
	[Parameter] public RenderFragment<RenderFragment> GroupTemplate { get; set; }
	[Parameter] public RenderFragment ItemPlaceHolder { get; set; }

	/// <summary>
	/// The layout of the ListView, a set of HTML to contain the repeated elements of the ItemTemplate and AlternativeItemTemplate 
	/// </summary>
	[Parameter]
	public RenderFragment<RenderFragment> LayoutTemplate { get; set; }

	/// <summary>
	/// Triggers when the layout template is completed construction 
	/// </summary>
	[Parameter]
	public EventHandler OnLayoutCreated { get; set; }

	#endregion

	[Parameter] public int GroupItemCount { get; set; } = 0;

	[Parameter] // TODO: Implement
	public InsertItemPosition InsertItemPosition { get; set; }

	[Parameter] // TODO: Implement
	public int SelectedIndex { get; set; }

	/// <summary>
	/// Style is not applied by this control
	/// </summary>
	[Parameter, Obsolete("Style is not applied by this control")]
	public string Style { get; set; }


	[Parameter] public RenderFragment ChildContent { get; set; }

	[CascadingParameter(Name = "Host")] public BaseWebFormsComponent HostComponent { get; set; }

	protected override void OnInitialized()
	{
	  HostComponent = this;
	  base.OnInitialized();
	}
  }
}
