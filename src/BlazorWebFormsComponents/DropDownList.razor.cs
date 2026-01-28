using BlazorComponentUtilities;
using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a drop-down list control that allows the user to select a single item from a list.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the data source.</typeparam>
	public partial class DropDownList<TItem> : DataBoundComponent<TItem>, IStyle
	{
		/// <summary>
		/// Gets or sets the collection of list items in the DropDownList.
		/// </summary>
		[Parameter]
		public ListItemCollection StaticItems { get; set; } = new();

		/// <summary>
		/// Gets or sets the selected value.
		/// </summary>
		[Parameter]
		public string SelectedValue { get; set; }

		/// <summary>
		/// Gets or sets the event callback that is invoked when the selected value changes.
		/// </summary>
		[Parameter]
		public EventCallback<string> SelectedValueChanged { get; set; }

		/// <summary>
		/// Gets or sets the zero-based index of the selected item.
		/// </summary>
		[Parameter]
		public int SelectedIndex { get; set; } = -1;

		/// <summary>
		/// Gets or sets the event callback that is invoked when the selected index changes.
		/// </summary>
		[Parameter]
		public EventCallback<int> SelectedIndexChanged { get; set; }

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
		/// Gets or sets the event callback that is invoked when the selected index changes.
		/// </summary>
		[Parameter]
		public EventCallback<ChangeEventArgs> OnSelectedIndexChanged { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the control automatically posts back to the server when the selection changes.
		/// This property is obsolete in Blazor and is included for compatibility only.
		/// </summary>
		[Parameter, Obsolete("AutoPostBack is not supported in Blazor. Use OnSelectedIndexChanged event instead.")]
		public bool AutoPostBack { get; set; }

		/// <summary>
		/// Gets the currently selected item.
		/// </summary>
		public ListItem SelectedItem => GetItems().FirstOrDefault(i => i.Value == SelectedValue);

		// IStyle implementation
		[Parameter]
		public WebColor BackColor { get; set; }

		[Parameter]
		public WebColor BorderColor { get; set; }

		[Parameter]
		public BorderStyle BorderStyle { get; set; }

		[Parameter]
		public Unit BorderWidth { get; set; }

		[Parameter]
		public string CssClass { get; set; }

		[Parameter]
		public FontInfo Font { get; set; } = new FontInfo();

		[Parameter]
		public WebColor ForeColor { get; set; }

		[Parameter]
		public Unit Height { get; set; }

		[Parameter]
		public Unit Width { get; set; }

		protected string Style => this.ToStyle().NullIfEmpty();

		private async Task HandleChange(ChangeEventArgs e)
		{
			SelectedValue = e.Value?.ToString();
			await SelectedValueChanged.InvokeAsync(SelectedValue);

			var items = GetItems().ToList();
			SelectedIndex = items.FindIndex(i => i.Value == SelectedValue);
			await SelectedIndexChanged.InvokeAsync(SelectedIndex);

			await OnSelectedIndexChanged.InvokeAsync(e);
		}

		private IEnumerable<ListItem> GetItems()
		{
			// Return static Items first
			foreach (var item in StaticItems)
			{
				yield return item;
			}

			// Then data-bound items
			if (Items != null)
			{
				foreach (var dataItem in Items)
				{
					yield return new ListItem
					{
						Text = GetPropertyValue(dataItem, DataTextField),
						Value = GetPropertyValue(dataItem, DataValueField)
					};
				}
			}
		}

		private string GetPropertyValue(TItem item, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return item?.ToString() ?? string.Empty;

			var prop = typeof(TItem).GetProperty(propertyName);
			return prop?.GetValue(item)?.ToString() ?? string.Empty;
		}
	}
}
