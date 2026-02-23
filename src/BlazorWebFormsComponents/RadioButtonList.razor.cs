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
	public partial class RadioButtonList<TItem> : BaseListControl<TItem>
	{
		private string _groupName = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Gets or sets the number of columns to display in the list control.
		/// </summary>
		[Parameter]
		public int RepeatColumns { get; set; } = 0;

		/// <summary>
		/// Gets or sets the direction in which the radio buttons are displayed.
		/// </summary>
		[Parameter]
		public DataListEnum RepeatDirection { get; set; } = DataListEnum.Vertical;

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

		private async Task HandleChange(ListItem item, ChangeEventArgs e)
		{
			SelectedValue = item.Value;
			await SelectedValueChanged.InvokeAsync(SelectedValue);

			var items = GetItems().ToList();
			SelectedIndex = items.FindIndex(i => i.Value == SelectedValue);
			await SelectedIndexChanged.InvokeAsync(SelectedIndex);

			await OnSelectedIndexChanged.InvokeAsync(e);
		}

		private string GetInputId(int index) => $"{_groupName}_{index}";
	}
}
