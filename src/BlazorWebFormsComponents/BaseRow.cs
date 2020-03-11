using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Base abstract row class
	/// </summary>
  public abstract class BaseRow : BaseWebFormsComponent, IRow
  {
		///<inheritdoc/>
		[CascadingParameter(Name = "RowCollection")] public IRowCollection RowCollection { get; set; }

		///<inheritdoc/>
		[Parameter] public object DataItem { get; set; }

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
