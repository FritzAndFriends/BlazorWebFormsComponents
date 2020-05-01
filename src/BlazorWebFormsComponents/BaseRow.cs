using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Base abstract row class
	/// </summary>
  public abstract class BaseRow<ItemType> : BaseWebFormsComponent, IRow<ItemType>
  {
		///<inheritdoc/>
		[CascadingParameter(Name = "RowCollection")] public IRowCollection<ItemType> RowCollection { get; set; }

		///<inheritdoc/>
		[Parameter] public ItemType DataItem { get; set; }

		public void Dispose()
		{
			RowCollection.RemoveRow(this);
		}

		///<inheritdoc/>
		protected override void OnInitialized()
		{
			RowCollection.AddRow(this);
		}
	}
}
