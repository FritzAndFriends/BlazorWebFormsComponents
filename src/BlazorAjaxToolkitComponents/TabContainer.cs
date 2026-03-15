using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// A container component that displays content in tabbed panels.
/// Only the active tab's content is visible. Clicking a tab header switches the active tab.
/// Emulates the Ajax Control Toolkit TabContainer control.
/// </summary>
public class TabContainer : ComponentBase, IAsyncDisposable
{
	private readonly string _containerId = $"act-tabcontainer-{Guid.NewGuid():N}";
	private readonly List<TabPanel> _panels = new();
	private IJSObjectReference _module;
	private bool _disposed;
	private bool _jsInitialized;

	[Inject]
	private IJSRuntime JS { get; set; } = default!;

	/// <summary>
	/// The zero-based index of the currently active tab.
	/// </summary>
	[Parameter]
	public int ActiveTabIndex { get; set; }

	/// <summary>
	/// Raised when the active tab changes.
	/// </summary>
	[Parameter]
	public EventCallback<int> OnActiveTabChanged { get; set; }

	/// <summary>
	/// Name of a client-side JavaScript function to invoke when the active tab changes.
	/// The function receives the new tab index as its argument.
	/// </summary>
	[Parameter]
	public string OnClientActiveTabChanged { get; set; } = string.Empty;

	/// <summary>
	/// Specifies which scrollbars to display on the tab content area.
	/// </summary>
	[Parameter]
	public ScrollBars ScrollBars { get; set; } = ScrollBars.None;

	/// <summary>
	/// CSS class applied to the outer TabContainer div.
	/// </summary>
	[Parameter]
	public string CssClass { get; set; } = string.Empty;

	/// <summary>
	/// Child content containing <see cref="TabPanel"/> components.
	/// </summary>
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	/// <summary>
	/// Gets the unique DOM element ID for this TabContainer.
	/// </summary>
	internal string ContainerId => _containerId;

	/// <summary>
	/// Gets the number of registered tab panels.
	/// </summary>
	internal int PanelCount => _panels.Count;

	internal void AddPanel(TabPanel panel)
	{
		if (!_panels.Contains(panel))
		{
			_panels.Add(panel);
			StateHasChanged();
		}
	}

	internal void RemovePanel(TabPanel panel)
	{
		if (_panels.Remove(panel))
		{
			if (ActiveTabIndex >= _panels.Count && _panels.Count > 0)
			{
				ActiveTabIndex = _panels.Count - 1;
			}
			StateHasChanged();
		}
	}

	internal int GetPanelIndex(TabPanel panel) => _panels.IndexOf(panel);

	internal bool IsTabActive(TabPanel panel) => GetPanelIndex(panel) == ActiveTabIndex;

	internal async Task SelectTabAsync(int index)
	{
		if (index < 0 || index >= _panels.Count) return;
		if (!_panels[index].Enabled) return;
		if (index == ActiveTabIndex) return;

		ActiveTabIndex = index;
		await OnActiveTabChanged.InvokeAsync(ActiveTabIndex);

		if (_jsInitialized && !string.IsNullOrEmpty(OnClientActiveTabChanged))
		{
			await InvokeClientCallbackAsync(index);
		}

		StateHasChanged();
	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		// Outer container
		builder.OpenElement(0, "div");
		builder.AddAttribute(1, "id", _containerId);
		builder.AddAttribute(2, "class", string.IsNullOrEmpty(CssClass)
			? "ajax__tab_container"
			: $"ajax__tab_container {CssClass}");

		// Tab header strip
		builder.OpenElement(3, "div");
		builder.AddAttribute(4, "class", "ajax__tab_header");
		builder.AddAttribute(5, "role", "tablist");

		for (var i = 0; i < _panels.Count; i++)
		{
			var panel = _panels[i];
			var index = i;
			var isActive = i == ActiveTabIndex;
			var tabClass = isActive ? "ajax__tab_active" : "ajax__tab_inactive";
			if (!panel.Enabled) tabClass += " ajax__tab_disabled";

			builder.OpenElement(6, "span");
			builder.AddAttribute(7, "class", tabClass);
			builder.AddAttribute(8, "role", "tab");
			builder.AddAttribute(9, "aria-selected", isActive);
			builder.AddAttribute(10, "data-tab-index", index);

			if (panel.Enabled)
			{
				builder.AddAttribute(11, "onclick",
					EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectTabAsync(index)));
				builder.AddAttribute(12, "style", "cursor:pointer;");
			}

			// Tab header content: template or text
			if (panel.HeaderTemplate != null)
			{
				builder.AddContent(13, panel.HeaderTemplate);
			}
			else
			{
				builder.AddContent(14, panel.HeaderText ?? string.Empty);
			}

			builder.CloseElement(); // span
		}

		builder.CloseElement(); // tab header div

		// Tab body area with scrollbar support
		builder.OpenElement(15, "div");
		builder.AddAttribute(16, "class", "ajax__tab_body");
		var bodyStyle = GetScrollBarStyle();
		if (!string.IsNullOrEmpty(bodyStyle))
		{
			builder.AddAttribute(17, "style", bodyStyle);
		}

		// CascadingValue for child TabPanels
		builder.OpenComponent<CascadingValue<TabContainer>>(18);
		builder.AddComponentParameter(19, "Value", this);
		builder.AddComponentParameter(20, "ChildContent", ChildContent);
		builder.CloseComponent();

		builder.CloseElement(); // tab body div
		builder.CloseElement(); // outer container div
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !string.IsNullOrEmpty(OnClientActiveTabChanged))
		{
			try
			{
				_module = await JS.InvokeAsync<IJSObjectReference>(
					"import",
					"./_content/BlazorAjaxToolkitComponents/js/tab-container.js");

				_jsInitialized = true;
			}
			catch (JSException ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"[TabContainer] JS init skipped (SSR): {ex.Message}");
			}
			catch (JSDisconnectedException) { }
		}
	}

	private async Task InvokeClientCallbackAsync(int tabIndex)
	{
		if (_module == null) return;

		try
		{
			await _module.InvokeVoidAsync("invokeClientCallback", OnClientActiveTabChanged, tabIndex);
		}
		catch (JSException) { }
		catch (JSDisconnectedException) { }
	}

	private string GetScrollBarStyle()
	{
		return ScrollBars switch
		{
			ScrollBars.Horizontal => "overflow-x:auto;overflow-y:hidden;",
			ScrollBars.Vertical => "overflow-x:hidden;overflow-y:auto;",
			ScrollBars.Both => "overflow:auto;",
			ScrollBars.Auto => "overflow:auto;",
			_ => string.Empty
		};
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (_disposed) return;
		_disposed = true;

		if (_module != null)
		{
			try { await _module.DisposeAsync(); }
			catch (JSDisconnectedException) { }
			catch (ObjectDisposedException) { }
			_module = null;
		}

		GC.SuppressFinalize(this);
	}
}
