using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class FormView<ItemType> : DataBoundComponent<ItemType>, IFormViewStyleContainer, IPagerSettingsContainer where ItemType : class, new()
	{

		private static readonly Dictionary<string, FormViewMode> CommandNameModeLookup = new Dictionary<string, FormViewMode> {
			{ "delete", FormViewMode.ReadOnly },
			{ "edit", FormViewMode.Edit },
			{ "insert", FormViewMode.ReadOnly },
			{ "new", FormViewMode.Insert },
			{ "update", FormViewMode.ReadOnly }
		};

		public FormView()
		{

			// Listen for bubbled events
			BubbledEvent += FormView_BubbledEvent;

		}

		[Parameter]
		public RenderFragment<ItemType> EditItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> InsertItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text displayed in the header row.
		/// </summary>
		[Parameter]
		public string HeaderText { get; set; }

		/// <summary>
		/// Gets or sets the custom header template. Takes precedence over HeaderText.
		/// </summary>
		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text displayed in the footer row.
		/// </summary>
		[Parameter]
		public string FooterText { get; set; }

		/// <summary>
		/// Gets or sets the custom footer template. Takes precedence over FooterText.
		/// </summary>
		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text shown when the DataSource is empty or null.
		/// </summary>
		[Parameter]
		public string EmptyDataText { get; set; }

		/// <summary>
		/// Gets or sets the custom template for the empty state. Takes precedence over EmptyDataText.
		/// </summary>
		[Parameter]
		public RenderFragment EmptyDataTemplate { get; set; }

		/// <summary>
		/// Gets or sets the pager template. When set, replaces the default numeric pager.
		/// </summary>
		[Parameter]
		public RenderFragment PagerTemplate { get; set; }

		/// <summary>
		/// Gets or sets the caption text rendered in a caption element at the top of the table.
		/// </summary>
		[Parameter]
		public string Caption { get; set; }

		/// <summary>
		/// Gets or sets the horizontal or vertical position of the caption element.
		/// </summary>
		[Parameter]
		public TableCaptionAlign CaptionAlign { get; set; } = TableCaptionAlign.NotSet;


		public ItemType CurrentItem { get; set; }

		public FormViewMode CurrentMode { get; private set; }

		[Parameter]
		public FormViewMode DefaultMode { get; set; } = FormViewMode.ReadOnly;

		private int _Position = 1;
		protected int Position
		{
			get { return _Position; }
			set
			{
				_Position = value;
				CurrentItem = Items.Skip(value - 1).FirstOrDefault();
			}
		}

		/// <summary>
		/// Gets the CSS style for the caption element based on CaptionAlign.
		/// </summary>
		protected string GetCaptionStyle()
		{
			return CaptionAlign switch
			{
				TableCaptionAlign.Top => "caption-side:top",
				TableCaptionAlign.Bottom => "caption-side:bottom",
				TableCaptionAlign.Left => "text-align:left",
				TableCaptionAlign.Right => "text-align:right",
				_ => null
			};
		}

		/// <summary>
		/// Gets the effective row style based on the current mode.
		/// </summary>
		protected TableItemStyle GetCurrentRowStyle()
		{
			return CurrentMode switch
			{
				FormViewMode.Edit when EditRowStyle != null => EditRowStyle,
				FormViewMode.Insert when InsertRowStyle != null => InsertRowStyle,
				_ => RowStyle
			};
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			if (firstRender)
			{
				if ((CurrentItem is null) && Items != null && Items.Any()) Position = 1;
				await ItemCreated.InvokeAsync();
			}

			await base.OnAfterRenderAsync(firstRender);

		}

		#region TableItemStyle Properties (IFormViewStyleContainer)

		public TableItemStyle RowStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle EditRowStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle InsertRowStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle HeaderStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle FooterStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle EmptyDataRowStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle PagerStyle { get; internal set; } = new TableItemStyle();

		#endregion

		#region Style RenderFragment Parameters

		[Parameter] public RenderFragment RowStyleContent { get; set; }
		[Parameter] public RenderFragment EditRowStyleContent { get; set; }
		[Parameter] public RenderFragment InsertRowStyleContent { get; set; }
		[Parameter] public RenderFragment HeaderStyleContent { get; set; }
		[Parameter] public RenderFragment FooterStyleContent { get; set; }
		[Parameter] public RenderFragment EmptyDataRowStyleContent { get; set; }
		[Parameter] public RenderFragment PagerStyleContent { get; set; }

		#endregion

		#region PagerSettings

		/// <summary>
		/// Gets the pager settings for the FormView.
		/// </summary>
		public PagerSettings PagerSettings { get; internal set; } = new PagerSettings();

		/// <summary>
		/// Content for the PagerSettings sub-component.
		/// </summary>
		[Parameter] public RenderFragment PagerSettingsContent { get; set; }

		#endregion

		#region FormView Events

		protected override Task OnInitializedAsync()
		{

			CurrentMode = DefaultMode;

			return base.OnInitializedAsync();
		}

		[Parameter]
		public EventCallback<FormViewModeEventArgs> ModeChanging { get; set; }

		/// <summary>
		/// Occurs after the mode of the control has changed.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewModeEventArgs> ModeChanged { get; set; }

		/// <summary>
		/// Occurs when a command button within the FormView is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewCommandEventArgs> ItemCommand { get; set; }

		/// <summary>
		/// Occurs when the FormView control is first created and data-bound.
		/// </summary>
		[Parameter]
		public EventCallback ItemCreated { get; set; }

		/// <summary>
		/// Occurs when the page index is changing. Supports cancellation.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> PageIndexChanging { get; set; }

		/// <summary>
		/// Occurs after the page index has changed.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> PageIndexChanged { get; set; }

		private void FormView_BubbledEvent(object sender, System.EventArgs e)
		{

			if (e is CommandEventArgs args) HandleCommandArgs(args);

		}

		private void HandleCommandArgs(CommandEventArgs args)
		{
			// Fire ItemCommand for all commands
			var commandArgs = new FormViewCommandEventArgs(args);
			commandArgs.Sender = this;
			ItemCommand.InvokeAsync(commandArgs).GetAwaiter().GetResult();

			switch (args.CommandName.ToLowerInvariant())
			{
				case "cancel":
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					ModeChanged.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					break;
				case "edit":
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Edit;
					ModeChanged.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit, Sender = this }).GetAwaiter().GetResult();
					break;
				case "delete":
					Exception caughtException = null;
					try {
						OnItemDeleting.InvokeAsync(new FormViewDeleteEventArgs(Position) { Sender = this }).GetAwaiter().GetResult();
					} catch (Exception ex) {
						caughtException = ex;
					}
					// do we do the deletion?
					OnItemDeleted.InvokeAsync(new FormViewDeletedEventArgs(Position, caughtException) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					Position = (Position == 0) ? 0 : Position - 1;
					break;
				case "insert":
					OnItemInserting.InvokeAsync(new FormViewInsertEventArgs("insert") { Sender = this }).GetAwaiter().GetResult();
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Insert, Sender = this }).GetAwaiter().GetResult();
					OnItemInserted.InvokeAsync(new FormViewInsertEventArgs("insert") { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Insert;
					ModeChanged.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Insert, Sender = this }).GetAwaiter().GetResult();
					break;
				case "update":
					OnItemUpdating.InvokeAsync(new FormViewUpdateEventArgs("update") { Sender = this }).GetAwaiter().GetResult();
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					OnItemUpdated.InvokeAsync(new FormViewUpdatedEventArgs(0, null) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					ModeChanged.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					break;
				case "page":
					HandlePageCommand(args);
					break;

			}

			StateHasChanged();

		}

		private void HandlePageCommand(CommandEventArgs args)
		{
			if (Items == null) return;

			var totalItems = Items.Count();
			var oldPosition = Position;
			int newPosition;

			if (args.CommandArgument is string pageArg)
			{
				newPosition = pageArg.ToLowerInvariant() switch
				{
					"next" => Math.Min(Position + 1, totalItems),
					"prev" => Math.Max(Position - 1, 1),
					"first" => 1,
					"last" => totalItems,
					_ => int.TryParse(pageArg, out var p) ? p : Position
				};
			}
			else
			{
				newPosition = Position;
			}

			var pageArgs = new PageChangedEventArgs(newPosition - 1, oldPosition - 1, totalItems, newPosition - 1);
			PageIndexChanging.InvokeAsync(pageArgs).GetAwaiter().GetResult();

			if (pageArgs.Cancel) return;

			Position = pageArgs.NewPageIndex + 1;
			PageIndexChanged.InvokeAsync(pageArgs).GetAwaiter().GetResult();
		}

		#endregion

		#region Custom Events

		[Parameter]
		public EventCallback<FormViewDeleteEventArgs> OnItemDeleting { get; set; }

		[Parameter]
		public EventCallback<FormViewDeletedEventArgs> OnItemDeleted { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> OnItemInserting { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> OnItemInserted { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdateEventArgs> OnItemUpdating { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdatedEventArgs> OnItemUpdated { get; set; }

		#endregion

	}

}
