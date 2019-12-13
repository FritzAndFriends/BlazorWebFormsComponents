using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

  public partial class ListView<ItemType> : BaseWebFormsComponent
  {

    public ListView()
    {
    }

    [Parameter]
    public IEnumerable<ItemType> Items { get; set; }

    [Parameter]
    public IEnumerable<ItemType> DataSource
    {
      get { return Items; }
      set { 
        Items = value;
        this.StateHasChanged();
      }
    }

    [Parameter]
    public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

    /// <summary>
    /// Defines the content to render if the data source returns no data.
    /// </summary>
    [Parameter]
    public RenderFragment EmptyDataTemplate { get; set; }

    [Parameter]
    public RenderFragment ItemSeparatorTemplate { get; set; }

    [Parameter]
    public RenderFragment<ItemType> ItemTemplate { get; set; }

    [Parameter]
    [Obsolete("The LayoutTemplate child element is not supported in Blazor.  Instead, wrap the ListView component with the desired layout")]
    public RenderFragment LayoutTemplate { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> AdditionalAttributes { get; set; }

    [Parameter] // TODO: Implement
    public InsertItemPosition InsertItemPosition { get; set; }

    [Parameter] // TODO: Implement
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Style is not applied by this control
    /// </summary>
    [Parameter, Obsolete("Style is not applied by this control")]
    public string Style { get; set; }

  }

}
