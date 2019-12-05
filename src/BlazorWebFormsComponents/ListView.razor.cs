using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

  public partial class ListView<TItem>
  {

    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public IEnumerable<TItem> DataSource
    {
      get { return Items; }
      set { Items = value; }
    }


    [Parameter]
    public RenderFragment EmptyDataTemplate { get; set; }

    [Parameter]
    public RenderFragment LayoutTemplate { get; set; }

    [Parameter]
    public RenderFragment TableHeader { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> AdditionalAttributes { get; set; }

    [Obsolete("This method doesn't do anything in Blazor")]
    public void DataBind() { }

  }

}
