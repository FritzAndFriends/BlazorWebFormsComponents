using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public abstract class BaseColumn<ItemType> : BaseWebFormsComponent, IColumn<ItemType>
	{
		///<inheritdoc/>
		[CascadingParameter(Name = "ColumnCollection")]
		public IColumnCollection<ItemType> ParentColumnsCollection { get; set; }

		///<inheritdoc/>
		[Parameter] public string FooterText { get; set; }

		///<inheritdoc/>
		[CascadingParameter(Name="FooterStyle")] public TableItemStyle FooterStyle { get; set; } = new TableItemStyle();

		///<inheritdoc/>
		[Parameter] public string HeaderText { get; set; }

		///<inheritdoc/>
		[CascadingParameter(Name="HeaderStyle")] public TableItemStyle HeaderStyle { get; set; } = new TableItemStyle();

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

		protected object[] GetDataFields(ItemType item, string dataFieldNames)
		{
			var dataFields = dataFieldNames.Split(',').Select(s => s.Trim()).ToList();
			var fields = new object[dataFields.Count];
			for (var i = 0; i < dataFields.Count; i++)
			{
				fields[i] = DataBinder.GetPropertyValue(item, dataFields[i]);
			}
			return fields;
		}
	}
}
