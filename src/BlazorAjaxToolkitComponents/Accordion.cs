using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// A container component that displays collapsible content panes arranged vertically.
/// Only one pane is expanded at a time (controlled by <see cref="SelectedIndex"/>).
/// Emulates the Ajax Control Toolkit Accordion control.
/// </summary>
public class Accordion : ComponentBase, IAsyncDisposable
{
	private readonly string _containerId = $"act-accordion-{Guid.NewGuid():N}";
	private readonly List<AccordionPane> _panes = new();
	private IJSObjectReference _module;
	private bool _disposed;
	private bool _jsInitialized;
	private int _previousSelectedIndex = -1;

	[Inject]
	private IJSRuntime JS { get; set; } = default!;

	/// <summary>
	/// The zero-based index of the currently selected (expanded) pane.
	/// Set to -1 when no pane is expanded (only valid when <see cref="RequireOpenedPane"/> is false).
	/// </summary>
	[Parameter]
	public int SelectedIndex { get; set; }

	/// <summary>
	/// Raised when the selected pane index changes.
	/// </summary>
	[Parameter]
	public EventCallback<int> SelectedIndexChanged { get; set; }

	/// <summary>
	/// Whether to use fade transitions when switching panes.
	/// </summary>
	[Parameter]
	public bool FadeTransitions { get; set; }

	/// <summary>
	/// Duration of expand/collapse animations in milliseconds. Default is 300.
	/// </summary>
	[Parameter]
	public int TransitionDuration { get; set; } = 300;

	/// <summary>
	/// CSS class applied to each pane header div.
	/// </summary>
	[Parameter]
	public string HeaderCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to each pane content div.
	/// </summary>
	[Parameter]
	public string ContentCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Whether at least one pane must always be open. Default is true.
	/// When false, clicking the selected pane header collapses it.
	/// </summary>
	[Parameter]
	public bool RequireOpenedPane { get; set; } = true;

	/// <summary>
	/// Specifies how pane content areas are automatically sized.
	/// </summary>
	[Parameter]
	public AutoSizeMode AutoSize { get; set; } = AutoSizeMode.None;

	/// <summary>
	/// Whether header clicks suppress postback behavior. Default is true.
	/// </summary>
	[Parameter]
	public bool SuppressHeaderPostbacks { get; set; } = true;

	/// <summary>
	/// CSS class applied to the outer Accordion container div.
	/// </summary>
	[Parameter]
	public string CssClass { get; set; } = string.Empty;

	/// <summary>
	/// Child content containing <see cref="AccordionPane"/> components.
	/// </summary>
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	/// <summary>
	/// Gets the unique DOM element ID for this Accordion container.
	/// </summary>
	internal string ContainerId => _containerId;

	/// <summary>
	/// Gets the number of registered panes.
	/// </summary>
	internal int PaneCount => _panes.Count;

	internal void AddPane(AccordionPane pane)
	{
		if (!_panes.Contains(pane))
		{
			_panes.Add(pane);
			StateHasChanged();
		}
	}

	internal void RemovePane(AccordionPane pane)
	{
		if (_panes.Remove(pane))
		{
			if (SelectedIndex >= _panes.Count && _panes.Count > 0)
			{
				SelectedIndex = _panes.Count - 1;
			}
			StateHasChanged();
		}
	}

	internal int GetPaneIndex(AccordionPane pane) => _panes.IndexOf(pane);

	internal bool IsPaneSelected(AccordionPane pane) => GetPaneIndex(pane) == SelectedIndex;

	internal async Task SelectPaneAsync(AccordionPane pane)
	{
		var index = GetPaneIndex(pane);
		if (index < 0) return;

		if (index == SelectedIndex && !RequireOpenedPane)
		{
			var oldIndex = SelectedIndex;
			SelectedIndex = -1;
			await AnimatePaneChangeAsync(oldIndex, -1);
		}
		else if (index != SelectedIndex)
		{
			var oldIndex = SelectedIndex;
			SelectedIndex = index;
			await AnimatePaneChangeAsync(oldIndex, index);
		}
		else
		{
			return;
		}

		await SelectedIndexChanged.InvokeAsync(SelectedIndex);
		StateHasChanged();
	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "div");
		builder.AddAttribute(1, "id", _containerId);
		builder.AddAttribute(2, "class", string.IsNullOrEmpty(CssClass) ? "ajax__accordion" : $"ajax__accordion {CssClass}");

		builder.OpenComponent<CascadingValue<Accordion>>(3);
		builder.AddComponentParameter(4, "Value", this);
		builder.AddComponentParameter(5, "ChildContent", ChildContent);
		builder.CloseComponent();

		builder.CloseElement();
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			try
			{
				_module = await JS.InvokeAsync<IJSObjectReference>(
					"import",
					"./_content/BlazorAjaxToolkitComponents/js/accordion.js");

				await _module.InvokeVoidAsync("initAccordion", _containerId, new
				{
					selectedIndex = SelectedIndex,
					transitionDuration = TransitionDuration,
					fadeTransitions = FadeTransitions,
					autoSize = (int)AutoSize
				});

				_jsInitialized = true;
				_previousSelectedIndex = SelectedIndex;
			}
			catch (JSException ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"[Accordion] JS init skipped (SSR): {ex.Message}");
			}
			catch (JSDisconnectedException) { }
		}
	}

	private async Task AnimatePaneChangeAsync(int oldIndex, int newIndex)
	{
		if (!_jsInitialized || _module == null) return;

		try
		{
			await _module.InvokeVoidAsync("selectPane", _containerId, oldIndex, newIndex, TransitionDuration, FadeTransitions);
		}
		catch (JSException) { }
		catch (JSDisconnectedException) { }
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (_disposed) return;
		_disposed = true;

		if (_module != null)
		{
			if (_jsInitialized)
			{
				try
				{
					await _module.InvokeVoidAsync("disposeAccordion", _containerId);
				}
				catch (JSException) { }
				catch (JSDisconnectedException) { }
				catch (ObjectDisposedException) { }
			}

			try { await _module.DisposeAsync(); }
			catch (JSDisconnectedException) { }
			catch (ObjectDisposedException) { }
			_module = null;
		}

		GC.SuppressFinalize(this);
	}
}
