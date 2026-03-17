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
		public EnumParameter<TableCaptionAlign> CaptionAlign { get; set; } = TableCaptionAlign.NotSet;

		/// <summary>
		/// Gets or sets whether the FormView renders an outer table element.
		/// When false, only the template content is rendered without a wrapping table.
		/// </summary>
		[Parameter]
		public bool RenderOuterTable { get; set; } = true;


		public ItemType CurrentItem { get; set; }

		public FormViewMode CurrentMode { get; private set; }

		[Parameter]
		public EnumParameter<FormViewMode> DefaultMode { get; set; } = FormViewMode.ReadOnly;

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
			return CaptionAlign.Value switch
			{
				TableCaptionAlign.Top => "caption-side:top",
				TableCaptionAlign.Bottom => "caption-side:bottom",
				TableCaptionAlign.Left => "text-align:left",
				TableCaptionAlign.Right => "text-align:right",
				_ => null
			};
		}

		/// <summary>
		/// Gets the combined table style, merging border-collapse:collapse with base style properties.
		/// </summary>
		protected string GetCombinedTableStyle()
		{
			var baseStyle = Style;
			return string.IsNullOrEmpty(baseStyle)
				? "border-collapse:collapse;"
				: $"border-collapse:collapse;{baseStyle}";
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
				var itemCreatedHandler = ItemCreated.HasDelegate ? ItemCreated : OnItemCreated;
				await itemCreatedHandler.InvokeAsync();
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
		/// Web Forms migration alias for ModeChanging.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewModeEventArgs> OnModeChanging { get; set; }

		/// <summary>
		/// Occurs after the mode of the control has changed.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewModeEventArgs> ModeChanged { get; set; }

		/// <summary>
		/// Web Forms migration alias for ModeChanged.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewModeEventArgs> OnModeChanged { get; set; }

		/// <summary>
		/// Occurs when a command button within the FormView is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewCommandEventArgs> ItemCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCommand.
		/// </summary>
		[Parameter]
		public EventCallback<FormViewCommandEventArgs> OnItemCommand { get; set; }

		/// <summary>
		/// Occurs when the FormView control is first created and data-bound.
		/// </summary>
		[Parameter]
		public EventCallback ItemCreated { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCreated.
		/// </summary>
		[Parameter]
		public EventCallback OnItemCreated { get; set; }

		/// <summary>
		/// Occurs when the page index is changing. Supports cancellation.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> PageIndexChanging { get; set; }

		/// <summary>
		/// Web Forms migration alias for PageIndexChanging.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> OnPageIndexChanging { get; set; }

		/// <summary>
		/// Occurs after the page index has changed.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> PageIndexChanged { get; set; }

		/// <summary>
		/// Web Forms migration alias for PageIndexChanged.
		/// </summary>
		[Parameter]
		public EventCallback<PageChangedEventArgs> OnPageIndexChanged { get; set; }

		private void FormView_BubbledEvent(object sender, System.EventArgs e)
		{

			if (e is CommandEventArgs args) HandleCommandArgs(args);

		}

		private void HandleCommandArgs(CommandEventArgs args)
		{
			// Fire ItemCommand for all commands
			var commandArgs = new FormViewCommandEventArgs(args);
			commandArgs.Sender = this;
			(ItemCommand.HasDelegate ? ItemCommand : OnItemCommand).InvokeAsync(commandArgs).GetAwaiter().GetResult();

			switch (args.CommandName.ToLowerInvariant())
			{
				case "cancel":
					(ModeChanging.HasDelegate ? ModeChanging : OnModeChanging).InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					(ModeChanged.HasDelegate ? ModeChanged : OnModeChanged).InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					break;
				case "edit":
					(ModeChanging.HasDelegate ? ModeChanging : OnModeChanging).InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Edit;
					(ModeChanged.HasDelegate ? ModeChanged : OnModeChanged).InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit, Sender = this }).GetAwaiter().GetResult();
					break;
				case "delete":
					Exception caughtException = null;
					try {
						(ItemDeleting.HasDelegate ? ItemDeleting : OnItemDeleting).InvokeAsync(new FormViewDeleteEventArgs(Position) { Sender = this }).GetAwaiter().GetResult();
					} catch (Exception ex) {
						caughtException = ex;
					}
					// do we do the deletion?
					(ItemDeleted.HasDelegate ? ItemDeleted : OnItemDeleted).InvokeAsync(new FormViewDeletedEventArgs(Position, caughtException) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					Position = (Position == 0) ? 0 : Position - 1;
					break;
				case "insert":
					(ItemInserting.HasDelegate ? ItemInserting : OnItemInserting).InvokeAsync(new FormViewInsertEventArgs("insert") { Sender = this }).GetAwaiter().GetResult();
					(ModeChanging.HasDelegate ? ModeChanging : OnModeChanging).InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Insert, Sender = this }).GetAwaiter().GetResult();
					(ItemInserted.HasDelegate ? ItemInserted : OnItemInserted).InvokeAsync(new FormViewInsertedEventArgs(0) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Insert;
					(ModeChanged.HasDelegate ? ModeChanged : OnModeChanged).InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Insert, Sender = this }).GetAwaiter().GetResult();
					break;
				case "update":
					(ItemUpdating.HasDelegate ? ItemUpdating : OnItemUpdating).InvokeAsync(new FormViewUpdateEventArgs("update") { Sender = this }).GetAwaiter().GetResult();
					(ModeChanging.HasDelegate ? ModeChanging : OnModeChanging).InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					(ItemUpdated.HasDelegate ? ItemUpdated : OnItemUpdated).InvokeAsync(new FormViewUpdatedEventArgs(0, null) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					(ModeChanged.HasDelegate ? ModeChanged : OnModeChanged).InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
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
			(PageIndexChanging.HasDelegate ? PageIndexChanging : OnPageIndexChanging).InvokeAsync(pageArgs).GetAwaiter().GetResult();

			if (pageArgs.Cancel) return;

			Position = pageArgs.NewPageIndex + 1;
			(PageIndexChanged.HasDelegate ? PageIndexChanged : OnPageIndexChanged).InvokeAsync(pageArgs).GetAwaiter().GetResult();
		}

		#endregion

		#region Custom Events

		[Parameter]
		public EventCallback<FormViewDeleteEventArgs> ItemDeleting { get; set; }

		[Parameter]
		public EventCallback<FormViewDeleteEventArgs> OnItemDeleting { get; set; }

		[Parameter]
		public EventCallback<FormViewDeletedEventArgs> ItemDeleted { get; set; }

		[Parameter]
		public EventCallback<FormViewDeletedEventArgs> OnItemDeleted { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> ItemInserting { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> OnItemInserting { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertedEventArgs> ItemInserted { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertedEventArgs> OnItemInserted { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdateEventArgs> ItemUpdating { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdateEventArgs> OnItemUpdating { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdatedEventArgs> ItemUpdated { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdatedEventArgs> OnItemUpdated { get; set; }

		#endregion

	}

}
