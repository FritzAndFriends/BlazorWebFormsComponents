using System;
using System.Collections.Generic;
using System.Linq;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides paging functionality for data-bound controls like ListView.
	/// </summary>
	public partial class DataPager : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the total number of rows in the data source.
		/// </summary>
		[Parameter]
		public int TotalRowCount { get; set; }

		/// <summary>
		/// Gets or sets the number of records to display on each page.
		/// </summary>
		[Parameter]
		public int PageSize { get; set; } = 10;

		/// <summary>
		/// Gets or sets the current page index (zero-based).
		/// </summary>
		[Parameter]
		public int PageIndex { get; set; } = 0;

		/// <summary>
		/// Gets or sets the callback for when PageIndex changes.
		/// </summary>
		[Parameter]
		public EventCallback<int> PageIndexChanged { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of page buttons to display.
		/// </summary>
		[Parameter]
		public int PageButtonCount { get; set; } = 5;

		/// <summary>
		/// Gets or sets the type of pager buttons to display.
		/// </summary>
		[Parameter]
		public PagerButtons Mode { get; set; } = PagerButtons.Numeric;

		/// <summary>
		/// Gets or sets the text for the First page button.
		/// </summary>
		[Parameter]
		public string FirstPageText { get; set; } = "First";

		/// <summary>
		/// Gets or sets the text for the Previous page button.
		/// </summary>
		[Parameter]
		public string PreviousPageText { get; set; } = "Previous";

		/// <summary>
		/// Gets or sets the text for the Next page button.
		/// </summary>
		[Parameter]
		public string NextPageText { get; set; } = "Next";

		/// <summary>
		/// Gets or sets the text for the Last page button.
		/// </summary>
		[Parameter]
		public string LastPageText { get; set; } = "Last";

		/// <summary>
		/// Gets or sets whether to show the First and Last buttons.
		/// </summary>
		[Parameter]
		public bool ShowFirstLastButtons { get; set; } = true;

		/// <summary>
		/// Gets or sets whether to show the Previous and Next buttons.
		/// </summary>
		[Parameter]
		public bool ShowPreviousNextButtons { get; set; } = true;

		/// <summary>
		/// Gets or sets whether to show numeric page buttons.
		/// </summary>
		[Parameter]
		public bool ShowNumericButtons { get; set; } = true;

		/// <summary>
		/// Event raised when the page is about to change.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> OnPageIndexChanging { get; set; }

		/// <summary>
		/// Event raised after the page has changed.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> OnPageIndexChanged { get; set; }

		/// <summary>
		/// Gets or sets custom child content (for TemplatePagerField support).
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Gets the total number of pages.
		/// </summary>
		protected int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRowCount / PageSize) : 0;

		/// <summary>
		/// Gets whether the current page is the first page.
		/// </summary>
		protected bool IsFirstPage => PageIndex <= 0;

		/// <summary>
		/// Gets whether the current page is the last page.
		/// </summary>
		protected bool IsLastPage => PageIndex >= TotalPages - 1;

		/// <summary>
		/// Gets the start row index for the current page.
		/// </summary>
		public int StartRowIndex => PageIndex * PageSize;

		/// <summary>
		/// Gets the maximum rows for the current page.
		/// </summary>
		public int MaximumRows => PageSize;

		/// <summary>
		/// Gets the range of page numbers to display.
		/// </summary>
		protected IEnumerable<int> GetPageRange()
		{
			if (TotalPages <= 0)
				return Enumerable.Empty<int>();

			// Calculate the start and end of the page button range
			var halfButtons = PageButtonCount / 2;
			var startPage = Math.Max(0, PageIndex - halfButtons);
			var endPage = Math.Min(TotalPages - 1, startPage + PageButtonCount - 1);

			// Adjust start if we're near the end
			if (endPage - startPage < PageButtonCount - 1)
			{
				startPage = Math.Max(0, endPage - PageButtonCount + 1);
			}

			return Enumerable.Range(startPage, endPage - startPage + 1);
		}

		/// <summary>
		/// Navigates to the specified page.
		/// </summary>
		protected async void GoToPage(int newPageIndex)
		{
			if (newPageIndex < 0 || newPageIndex >= TotalPages || newPageIndex == PageIndex)
				return;

			var args = new PageChangedEventArgs(newPageIndex, PageIndex, TotalPages, newPageIndex * PageSize);

			// Raise changing event (allows cancellation)
			await OnPageIndexChanging.InvokeAsync(args);

			if (args.Cancel)
				return;

			// Update page index
			PageIndex = args.NewPageIndex;
			await PageIndexChanged.InvokeAsync(PageIndex);

			// Raise changed event
			await OnPageIndexChanged.InvokeAsync(args);

			StateHasChanged();
		}

		/// <summary>
		/// Goes to the first page.
		/// </summary>
		protected void GoToFirstPage() => GoToPage(0);

		/// <summary>
		/// Goes to the previous page.
		/// </summary>
		protected void GoToPreviousPage() => GoToPage(PageIndex - 1);

		/// <summary>
		/// Goes to the next page.
		/// </summary>
		protected void GoToNextPage() => GoToPage(PageIndex + 1);

		/// <summary>
		/// Goes to the last page.
		/// </summary>
		protected void GoToLastPage() => GoToPage(TotalPages - 1);

		/// <summary>
		/// Determines if First/Last buttons should be shown based on Mode.
		/// </summary>
		protected bool ShouldShowFirstLast =>
			ShowFirstLastButtons &&
			(Mode == PagerButtons.NextPreviousFirstLast || Mode == PagerButtons.NumericFirstLast);

		/// <summary>
		/// Determines if Previous/Next buttons should be shown based on Mode.
		/// </summary>
		protected bool ShouldShowPreviousNext =>
			ShowPreviousNextButtons &&
			(Mode == PagerButtons.NextPrevious || Mode == PagerButtons.NextPreviousFirstLast);

		/// <summary>
		/// Determines if numeric buttons should be shown based on Mode.
		/// </summary>
		protected bool ShouldShowNumeric =>
			ShowNumericButtons &&
			(Mode == PagerButtons.Numeric || Mode == PagerButtons.NumericFirstLast);
	}
}
