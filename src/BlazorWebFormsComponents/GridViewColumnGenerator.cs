using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// The GridView Column Generator
	/// </summary>
	public static class GridViewColumnGenerator
	{
		private const string IndexerPropertyName = "Item";

		/// <summary>
		/// Generate columns for a given GridView based on the properties of given Type
		/// </summary>
		/// <typeparam name="ItemType"> The type </typeparam>
		/// <param name="gridView"> The GridView </param>
		public static void GenerateColumns<ItemType>(GridView<ItemType> gridView)
		{
			var type = typeof(ItemType);
			var propertiesInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			if (propertiesInfo.Count() == 0)
			{
				propertiesInfo = (gridView.DataSource as IEnumerable<ItemType>).First()?.GetType()
					.GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Enumerable.Empty<PropertyInfo>()
					.ToArray();
			}

			foreach (var propertyInfo in propertiesInfo.Where(p => p.Name != IndexerPropertyName).OrderBy(x => x.MetadataToken))
			{
				var newColumn = new BoundField<ItemType>
				{
					DataField = propertyInfo.Name,
					ParentColumnsCollection = gridView,
					HeaderText = propertyInfo.Name
				};
				gridView.ColumnList.Add(newColumn);
			}
		}
	}
}
