using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.GridView
{
  public static class GridViewColumnGenerator
  {
		public static void AutogenerateColumns<ItemType>(GridView<ItemType> gridView)
		{
			var type = typeof(ItemType);
			var propertiesInfo = type.GetProperties();
			foreach (var propertyInfo in propertiesInfo)
			{
				var newColumn = new BoundField<ItemType> {
					DataField = propertyInfo.Name,
					GridView = gridView,
					HeaderText = propertyInfo.Name
				};
				gridView.GridColumns.Add(newColumn);
			}
		}
	}
}
