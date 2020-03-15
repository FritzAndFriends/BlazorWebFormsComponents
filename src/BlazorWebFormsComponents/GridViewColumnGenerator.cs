﻿namespace BlazorWebFormsComponents
{
	/// <summary>
	/// The GridView Column Generator
	/// </summary>
  public static class GridViewColumnGenerator
  {
		/// <summary>
		/// Generate columns for a given GridView based on the properties of given Type
		/// </summary>
		/// <typeparam name="ItemType"> The type </typeparam>
		/// <param name="gridView"> The GridView </param>
		public static void GenerateColumns<ItemType>(GridView<ItemType> gridView)
		{
			var type = typeof(ItemType);
			var propertiesInfo = type.GetProperties();
			foreach (var propertyInfo in propertiesInfo)
			{
				var newColumn = new BoundField {
					DataField = propertyInfo.Name,
					ParentColumnsCollection = gridView,
					HeaderText = propertyInfo.Name
				};
				gridView.ColumnList.Add(newColumn);
			}
		}
	}
}
