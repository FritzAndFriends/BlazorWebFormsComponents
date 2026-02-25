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
public partial class DropDownList<TItem> : BaseListControl<TItem>
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
/// Gets or sets a value indicating whether the control automatically posts back to the server when the selection changes.
/// This property is obsolete in Blazor and is included for compatibility only.
/// </summary>
[Parameter, Obsolete("AutoPostBack is not supported in Blazor. Use OnSelectedIndexChanged event instead.")]
public bool AutoPostBack { get; set; }

/// <summary>
/// Gets the currently selected item.
/// </summary>
public ListItem SelectedItem => GetItems().FirstOrDefault(i => i.Value == SelectedValue);

private async Task HandleChange(ChangeEventArgs e)
{
SelectedValue = e.Value?.ToString();
await SelectedValueChanged.InvokeAsync(SelectedValue);

var items = GetItems().ToList();
SelectedIndex = items.FindIndex(i => i.Value == SelectedValue);
await SelectedIndexChanged.InvokeAsync(SelectedIndex);

await OnSelectedIndexChanged.InvokeAsync(e);
}
}
}