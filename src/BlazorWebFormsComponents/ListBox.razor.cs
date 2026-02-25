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
	/// Represents a list box control that allows the user to select one or more items from a list.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the data source.</typeparam>
	public partial class ListBox<TItem> : BaseListControl<TItem>
	{
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
		/// Gets or sets the collection of selected values (for multiple selection mode).
		/// </summary>
		[Parameter]
		public List<string> SelectedValues { get; set; } = new();

		/// <summary>
		/// Gets or sets the event callback that is invoked when the selected values change.
		/// </summary>
		[Parameter]
		public EventCallback<List<string>> SelectedValuesChanged { get; set; }

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
		/// Gets or sets the number of rows displayed in the ListBox control.
		/// </summary>
		[Parameter]
		public int Rows { get; set; } = 4;

		/// <summary>
		/// Gets or sets the selection mode of the ListBox control.
		/// </summary>
		[Parameter]
		public ListSelectionMode SelectionMode { get; set; } = ListSelectionMode.Single;

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

		/// <summary>
		/// Gets the collection of selected items (for multiple selection mode).
		/// </summary>
		public IEnumerable<ListItem> SelectedItems =>
			GetItems().Where(i => SelectedValues.Contains(i.Value));

		private bool IsSelected(string value)
		{
			if (SelectionMode == ListSelectionMode.Multiple)
				return SelectedValues.Contains(value);
			return value == SelectedValue;
		}

		private async Task HandleChange(ChangeEventArgs e)
		{
			if (SelectionMode == ListSelectionMode.Multiple)
			{
				// For multiple selection, get all selected values
				var selectedValues = e.Value as string[] ?? new[] { e.Value?.ToString() };
				SelectedValues = selectedValues.Where(v => v != null).ToList();
				await SelectedValuesChanged.InvokeAsync(SelectedValues);
				SelectedValue = SelectedValues.FirstOrDefault();
			}
			else
			{
				SelectedValue = e.Value?.ToString();
				SelectedValues = string.IsNullOrEmpty(SelectedValue)
					? new List<string>()
					: new List<string> { SelectedValue };
			}

			await SelectedValueChanged.InvokeAsync(SelectedValue);

			var items = GetItems().ToList();
			SelectedIndex = items.FindIndex(i => i.Value == SelectedValue);
			await SelectedIndexChanged.InvokeAsync(SelectedIndex);

			await OnSelectedIndexChanged.InvokeAsync(e);
		}

	}
}
