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
	/// Represents a list control that displays a group of radio buttons for single selection.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the data source.</typeparam>
	public partial class RadioButtonList<TItem> : DataBoundComponent<TItem>, IStyle
	{
		private string _groupName = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Gets or sets the collection of static list items in the RadioButtonList.
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
		/// Gets or sets the number of columns to display in the list control.
		/// </summary>
		[Parameter]
		public int RepeatColumns { get; set; } = 0;

		/// <summary>
		/// Gets or sets the direction in which the radio buttons are displayed.
		/// </summary>
		[Parameter]
		public RepeatDirection RepeatDirection { get; set; } = RepeatDirection.Vertical;

		/// <summary>
		/// Gets or sets the layout of the radio buttons.
		/// </summary>
		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = Enums.RepeatLayout.Table;

		/// <summary>
		/// Gets or sets the alignment of the text label with respect to the radio button.
		/// </summary>
		[Parameter]
		public TextAlign TextAlign { get; set; } = TextAlign.Right;

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
		/// Gets or sets the event callback that is invoked when the selected index changes.
		/// </summary>
		[Parameter]
		public EventCallback<ChangeEventArgs> OnSelectedIndexChanged { get; set; }

		/// <summary>
		/// Gets or sets the amount of space between cells for table layout.
		/// </summary>
		[Parameter]
		public int CellPadding { get; set; } = -1;

		/// <summary>
		/// Gets or sets the amount of space between the contents of a cell and the cell's border for table layout.
		/// </summary>
		[Parameter]
		public int CellSpacing { get; set; } = -1;

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

		private async Task HandleChange(ListItem item, ChangeEventArgs e)
		{
			SelectedValue = item.Value;
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

		private string GetInputId(int index) => $"{_groupName}_{index}";
	}
}
