using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
  public abstract class BaseColumn : BaseWebFormsComponent, IColumn
	{
		///<inheritdoc/>
		[CascadingParameter(Name = "ColumnCollection")]
		public IColumnCollection ParentColumnsCollection { get; set; }

		///<inheritdoc/>
		[Parameter] public string HeaderText { get; set; }

		public void Dispose()
		{
			ParentColumnsCollection.RemoveColumn(this);
		}

		///<inheritdoc/>
		protected override void OnInitialized()
		{
			ParentColumnsCollection.AddColumn(this);
		}
	}
}
