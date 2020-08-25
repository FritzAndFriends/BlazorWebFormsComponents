using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
  public abstract class BaseColumn<ItemType> : BaseWebFormsComponent, IColumn<ItemType>
  {
	///<inheritdoc/>
	[CascadingParameter(Name = "ColumnCollection")]
	public IColumnCollection<ItemType> ParentColumnsCollection { get; set; }

	///<inheritdoc/>
	[Parameter] public string HeaderText { get; set; }

	public void Dispose()
	{
	  ParentColumnsCollection.RemoveColumn(this);
	}

	public abstract RenderFragment Render(ItemType item);

	///<inheritdoc/>
	protected override void OnInitialized()
	{
	  ParentColumnsCollection.AddColumn(this);
	}
  }
}
