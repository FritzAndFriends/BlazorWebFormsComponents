using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.DataBinding;

/// <summary>
/// Base class for list controls that display a collection of items (DropDownList, CheckBoxList, RadioButtonList, ListBox, BulletedList).
/// Emulates the ASP.NET Web Forms ListControl base class.
/// </summary>
/// <typeparam name="TItem">The type of items in the data source.</typeparam>
public class BaseListControl<TItem> : DataBoundComponent<TItem>
{
	/// <summary>
	/// Gets or sets the collection of static list items.
	/// </summary>
	[Parameter]
	public ListItemCollection StaticItems { get; set; } = new();

	/// <summary>
	/// Gets or sets the field of the data source that provides the text content of the list items.
	/// </summary>
	[Parameter]
	public string DataTextField { get; set; }

	/// <summary>
	/// Gets or sets the field of the data source that provides the value of each list item.
	/// </summary>
	[Parameter]
	public string DataValueField { get; set; }

	/// <summary>
	/// Gets or sets a formatting string used to control how data bound to the list control is displayed.
	/// For example, "{0:C}" formats item text as currency.
	/// </summary>
	[Parameter]
	public string DataTextFormatString { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether data-bound items are appended to statically defined items.
	/// When false (default), data-bound items replace static items. When true, they are appended.
	/// </summary>
	[Parameter]
	public bool AppendDataBoundItems { get; set; }

	/// <summary>
	/// Gets all items from both static and data-bound sources, applying format string and append logic.
	/// </summary>
	protected IEnumerable<ListItem> GetItems()
	{
		// Include static items when AppendDataBoundItems is true, or when there are no data-bound items
		if (AppendDataBoundItems || Items == null)
		{
			foreach (var item in StaticItems)
			{
				yield return FormatItem(item);
			}
		}

		// Then data-bound items
		if (Items != null)
		{
			foreach (var dataItem in Items)
			{
				var text = GetPropertyValue(dataItem, DataTextField);
				yield return new ListItem
				{
					Text = FormatText(text),
					Value = GetPropertyValue(dataItem, DataValueField)
				};
			}
		}
	}

	/// <summary>
	/// Gets the value of a property from a data item by property name.
	/// </summary>
	protected string GetPropertyValue(TItem item, string propertyName)
	{
		if (string.IsNullOrEmpty(propertyName))
			return item?.ToString() ?? string.Empty;

		var prop = typeof(TItem).GetProperty(propertyName);
		return prop?.GetValue(item)?.ToString() ?? string.Empty;
	}

	private string FormatText(string text)
	{
		if (!string.IsNullOrEmpty(DataTextFormatString))
			return string.Format(DataTextFormatString, text);
		return text;
	}

	private ListItem FormatItem(ListItem item)
	{
		if (string.IsNullOrEmpty(DataTextFormatString))
			return item;
		return new ListItem
		{
			Text = FormatText(item.Text),
			Value = item.Value,
			Selected = item.Selected,
			Enabled = item.Enabled
		};
	}
}
