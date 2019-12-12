using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

  public partial class ListView<ItemType>
  {

    public ListView()
    {
    }

    [Parameter]
    public IEnumerable<ItemType> Items { get; set; }

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

    [Obsolete("This method doesn't do anything in Blazor")]
    public void DataBind() { }

  }

}
