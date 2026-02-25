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
	/// Represents a list control that renders a group of checkboxes for multi-select scenarios.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the data source.</typeparam>
	public partial class CheckBoxList<TItem> : BaseListControl<TItem>
	{
		private string _baseId = Guid.NewGuid().ToString("N").Substring(0, 8);

		/// <summary>
		/// Gets or sets the number of columns to display in the CheckBoxList.
		/// </summary>
		[Parameter]
		public int RepeatColumns { get; set; } = 0;

		/// <summary>
		/// Gets or sets the direction in which the checkboxes are rendered (Vertical or Horizontal).
		/// </summary>
		[Parameter]
		public DataListEnum RepeatDirection { get; set; } = DataListEnum.Vertical;

		/// <summary>
		/// Gets or sets the layout of the checkboxes (Table, Flow, OrderedList, or UnorderedList).
		/// </summary>
		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = Enums.RepeatLayout.Table;

		/// <summary>
		/// Gets or sets the text alignment of the label relative to the checkbox (Left or Right).
		/// </summary>
		[Parameter]
		public TextAlign TextAlign { get; set; } = TextAlign.Right;

		/// <summary>
		/// Gets or sets the list of selected values.
		/// </summary>
		[Parameter]
		public List<string> SelectedValues { get; set; } = new();

		/// <summary>
		/// Gets or sets the event callback that is invoked when the selected values change.
		/// </summary>
		[Parameter]
		public EventCallback<List<string>> SelectedValuesChanged { get; set; }

		/// <summary>
		/// Gets or sets the cell padding for table layout.
		/// </summary>
		[Parameter]
		public int CellPadding { get; set; } = -1;

		/// <summary>
		/// Gets or sets the cell spacing for table layout.
		/// </summary>
		[Parameter]
		public int CellSpacing { get; set; } = -1;

		/// <summary>
		/// Gets or sets the event callback that is invoked when the selection changes.
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
		/// Gets the first selected value, or null if no items are selected.
		/// </summary>
		public string SelectedValue => SelectedValues.FirstOrDefault();

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		public IEnumerable<ListItem> SelectedItems => GetItems().Where(i => SelectedValues.Contains(i.Value));

		/// <summary>
		/// Gets the first selected item, or null if no items are selected.
		/// </summary>
		public ListItem SelectedItem => SelectedItems.FirstOrDefault();

		/// <summary>
		/// Gets the zero-based index of the first selected item, or -1 if no items are selected.
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				var items = GetItems().ToList();
				for (var i = 0; i < items.Count; i++)
				{
					if (SelectedValues.Contains(items[i].Value))
						return i;
				}
				return -1;
			}
		}

		private async Task HandleChange(ListItem item, ChangeEventArgs e)
		{
			var isChecked = (bool)e.Value;
			if (isChecked && !SelectedValues.Contains(item.Value))
			{
				SelectedValues.Add(item.Value);
			}
			else if (!isChecked && SelectedValues.Contains(item.Value))
			{
				SelectedValues.Remove(item.Value);
			}
			await SelectedValuesChanged.InvokeAsync(SelectedValues);
			await OnSelectedIndexChanged.InvokeAsync(e);
		}

	}
}
