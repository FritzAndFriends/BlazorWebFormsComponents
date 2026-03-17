using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Represents a single tab within a <see cref="TabContainer"/>.
/// Each tab has a header (text or template) and a content area.
/// </summary>
public class TabPanel : ComponentBase, IDisposable
{
	private bool _disposed;

	/// <summary>
	/// The parent <see cref="TabContainer"/>, provided via CascadingParameter.
	/// </summary>
	[CascadingParameter]
	public TabContainer ParentContainer { get; set; }

	/// <summary>
	/// The text displayed in this tab's header. Used when <see cref="HeaderTemplate"/> is not set.
	/// </summary>
	[Parameter]
	public string HeaderText { get; set; } = string.Empty;

	/// <summary>
	/// Custom header content for this tab. When set, takes precedence over <see cref="HeaderText"/>.
	/// </summary>
	[Parameter]
	public RenderFragment HeaderTemplate { get; set; }

	/// <summary>
	/// The content rendered in the tab panel body when this tab is active.
	/// </summary>
	[Parameter]
	public RenderFragment ContentTemplate { get; set; }

	/// <summary>
	/// Whether this tab can be selected. Default is true.
	/// Disabled tabs are rendered but cannot be clicked.
	/// </summary>
	[Parameter]
	public bool Enabled { get; set; } = true;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (ParentContainer == null)
		{
			throw new InvalidOperationException(
				$"{nameof(TabPanel)} must be placed inside a {nameof(TabContainer)} component.");
		}

		ParentContainer.AddPanel(this);
	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		var isActive = ParentContainer?.IsTabActive(this) ?? false;

		builder.OpenElement(0, "div");
		builder.AddAttribute(1, "class", "ajax__tab_panel");
		builder.AddAttribute(2, "role", "tabpanel");

		if (!isActive)
		{
			builder.AddAttribute(3, "style", "display:none;");
		}

		if (ContentTemplate != null)
		{
			builder.AddContent(4, ContentTemplate);
		}

		builder.CloseElement();
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			ParentContainer?.RemovePanel(this);
		}
	}
}
