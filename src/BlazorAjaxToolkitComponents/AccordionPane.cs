using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Represents a single collapsible pane within an <see cref="Accordion"/>.
/// Each pane has a clickable header and a content area that expands or collapses.
/// </summary>
public class AccordionPane : ComponentBase, IDisposable
{
	private readonly string _headerId = $"act-accordion-header-{Guid.NewGuid():N}";
	private readonly string _contentId = $"act-accordion-content-{Guid.NewGuid():N}";
	private bool _disposed;

	/// <summary>
	/// The parent <see cref="Accordion"/> container, provided via CascadingParameter.
	/// </summary>
	[CascadingParameter]
	public Accordion ParentAccordion { get; set; }

	/// <summary>
	/// The header content for this pane. Rendered inside the clickable header div.
	/// </summary>
	[Parameter]
	public RenderFragment Header { get; set; }

	/// <summary>
	/// The body content for this pane. Rendered inside the collapsible content div.
	/// </summary>
	[Parameter]
	public RenderFragment Content { get; set; }

	/// <summary>
	/// Gets the unique DOM element ID for this pane's header.
	/// </summary>
	internal string HeaderId => _headerId;

	/// <summary>
	/// Gets the unique DOM element ID for this pane's content area.
	/// </summary>
	internal string ContentId => _contentId;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (ParentAccordion == null)
		{
			throw new InvalidOperationException(
				$"{nameof(AccordionPane)} must be placed inside an {nameof(Accordion)} component.");
		}

		ParentAccordion.AddPane(this);
	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		var isSelected = ParentAccordion?.IsPaneSelected(this) ?? false;
		var paneIndex = ParentAccordion?.GetPaneIndex(this) ?? -1;
		var headerClass = ParentAccordion?.HeaderCssClass ?? string.Empty;
		var contentClass = ParentAccordion?.ContentCssClass ?? string.Empty;

		// Header div
		builder.OpenElement(0, "div");
		builder.AddAttribute(1, "id", _headerId);
		builder.AddAttribute(2, "class", string.IsNullOrEmpty(headerClass)
			? "ajax__accordion_header"
			: $"ajax__accordion_header {headerClass}");
		builder.AddAttribute(3, "role", "tab");
		builder.AddAttribute(4, "aria-selected", isSelected);
		builder.AddAttribute(5, "aria-expanded", isSelected);
		builder.AddAttribute(6, "data-pane-index", paneIndex);
		builder.AddAttribute(7, "style", "cursor:pointer;");
		builder.AddAttribute(8, "onclick",
			EventCallback.Factory.Create<MouseEventArgs>(this, OnHeaderClickAsync));
		builder.AddContent(9, Header);
		builder.CloseElement();

		// Content div
		builder.OpenElement(10, "div");
		builder.AddAttribute(11, "id", _contentId);
		builder.AddAttribute(12, "class", string.IsNullOrEmpty(contentClass)
			? "ajax__accordion_content"
			: $"ajax__accordion_content {contentClass}");
		builder.AddAttribute(13, "role", "tabpanel");
		builder.AddAttribute(14, "data-pane-index", paneIndex);

		if (!isSelected)
		{
			builder.AddAttribute(15, "style", "display:none;overflow:hidden;");
		}

		builder.AddContent(16, Content);
		builder.CloseElement();
	}

	private async Task OnHeaderClickAsync()
	{
		if (ParentAccordion != null)
		{
			await ParentAccordion.SelectPaneAsync(this);
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			ParentAccordion?.RemovePane(this);
		}
	}
}
