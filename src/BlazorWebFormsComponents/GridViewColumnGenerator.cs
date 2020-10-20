using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BlazorWebFormsComponents
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
			var props = TypeDescriptor.GetProperties(typeof(ItemType));
			if (props.Count == 0)
			{
				if (!(gridView.DataSource is IEnumerable<ItemType> gridDataEnumerable))
				{
					throw new InvalidOperationException($"Cannot auto generate columns for data type {gridView.DataSource?.GetType().FullName}.");
				}

				var firstDataItem = gridDataEnumerable.FirstOrDefault();
				if (firstDataItem == null)
				{
					throw new InvalidOperationException($"Cannot auto generate columns for data type {gridView.DataSource?.GetType().FullName} because there is no data.");
				}

				props = TypeDescriptor.GetProperties(firstDataItem);
			}

			foreach (var propertyInfo in props.OfType<PropertyDescriptor>().Where(p => IsBindableType(p.PropertyType)))
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

		private static bool IsBindableType(Type type, bool enableEnums = true)
		{
			if (type == null)
			{
				return false;
			}

			var underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}

			if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(Decimal) || type == typeof(Guid) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan))
			{
				return true;
			}
			else
			{
				return enableEnums && type.IsEnum;
			}
		}
	}
}
